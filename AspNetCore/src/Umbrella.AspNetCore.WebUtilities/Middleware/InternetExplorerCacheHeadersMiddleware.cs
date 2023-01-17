using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.Middleware;

/// <summary>
/// Middleware that examines the User-Agent string to detect the use of Internet Explorer
/// and applies caching headers to prevent certain reponses from being incorrectly cached.
/// </summary>
public class InternetExplorerCacheHeadersMiddleware
{
	#region Private Members
	private readonly RequestDelegate _next;
	private readonly ILogger _logger;
	private readonly InternetExplorerCacheHeadersMiddlewareOptions _options;
	#endregion

	#region Constructors
	/// <summary>
	/// Create a new instance.
	/// </summary>
	/// <param name="next">The next middleware to be executed.</param>
	/// <param name="logger">The <see cref="ILogger" />.</param>
	/// <param name="options">The options.</param>
	public InternetExplorerCacheHeadersMiddleware(
		RequestDelegate next,
		ILogger<InternetExplorerCacheHeadersMiddleware> logger,
		InternetExplorerCacheHeadersMiddlewareOptions options)
	{
		_next = next;
		_logger = logger;
		_options = options;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Invokes the middleware for the specified <see cref="HttpContext" />.
	/// </summary>
	/// <param name="context">The <see cref="HttpContext" />.</param>
	/// <returns>An awaitable <see cref="Task" />.</returns>
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			string method = context.Request.Method;
			string? userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

			bool addHeaders = !string.IsNullOrWhiteSpace(userAgent)
				&& (_options.UserAgentKeywords.Count == 0 || _options.UserAgentKeywords.Any(userAgent.Contains))
				&& (_options.Methods.Count == 0 || _options.Methods.Any(method.Contains));

			if (addHeaders)
			{
				context.Response.OnStarting(state =>
				{
					var response = (HttpResponse)state;

					string contentType = response.ContentType;

					// Apply the headers when no response content type - this may be the case when a 204 code is sent for a GET request that issues no content
					// If no content types have been specified in the options apply the headers to all response for the configured HTTP methods
					// If content types have been specified to filter on then only apply the headers for those responses
					if (string.IsNullOrWhiteSpace(contentType) || _options.ContentTypes.Count == 0 || _options.ContentTypes.Any(x => contentType.Contains(x, StringComparison.OrdinalIgnoreCase)))
					{
						// Set standard HTTP/1.0 no-cache header (no-store, no-cache, must-revalidate)
						// Set IE extended HTTP/1.1 no-cache headers (post-check, pre-check)
						response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

						// Set standard HTTP/1.0 no-cache header.
						response.Headers.Add("Pragma", "no-cache");

						// Set the Expires header.
						response.Headers.Add("Expires", "0");
					}

					return Task.CompletedTask;

				}, context.Response);
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