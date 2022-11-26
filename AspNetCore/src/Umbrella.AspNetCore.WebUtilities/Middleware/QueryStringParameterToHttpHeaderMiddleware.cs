using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.Middleware;

/// <summary>
/// Middleware that intercepts incoming HTTP requests and adds HTTP Headers with the same keys and values
/// as those found on the querystring.
/// </summary>
public class QueryStringParameterToHttpHeaderMiddleware
{
	#region Private Members
	private readonly RequestDelegate _next;
	private readonly ILogger _logger;
	private readonly QueryStringParameterToHttpHeaderMiddlewareOptions _options;
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="QueryStringParameterToHttpHeaderMiddleware" /> class.
	/// </summary>
	/// <param name="next">The next.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The options.</param>
	public QueryStringParameterToHttpHeaderMiddleware(
		RequestDelegate next,
		ILogger<QueryStringParameterToHttpHeaderMiddleware> logger,
		QueryStringParameterToHttpHeaderMiddlewareOptions options)
	{
		_next = next;
		_logger = logger;
		_options = options;
	}
	#endregion

	#region Middleware Members
	/// <summary>
	/// Invokes the middleware for the specified <see cref="HttpContext" />.
	/// </summary>
	/// <param name="context">The <see cref="HttpContext" />.</param>
	/// <returns>An awaitable <see cref="Task" />.</returns>
	public async Task Invoke(HttpContext context)
	{
		try
		{
			foreach (var kvp in _options.Mappings)
			{
				StringValues values = context.Request.Query[kvp.Key];

				if (!StringValues.IsNullOrEmpty(values))
				{
					string? value = values.FirstOrDefault();

					if (!string.IsNullOrWhiteSpace(value))
					{
						var headers = context.Request.GetTypedHeaders();

						if (_options.ValueTransformer is not null)
						{
							string newValue = _options.ValueTransformer(value);

							if (string.IsNullOrWhiteSpace(newValue))
								_logger.LogWarning($"The {nameof(QueryStringParameterToHttpHeaderMiddleware)} executed a value transformer which converted '{value}' to an empty string.");

							value = newValue;
						}

						headers.Set(kvp.Value, value);
					}
				}
			}

			await _next.Invoke(context);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw;
		}
	}
	#endregion
}