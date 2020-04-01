using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.WebUtilities.Middleware
{
	/// <summary>
	/// Middleware that intercepts incoming HTTP requests and adds HTTP Headers with the same keys and values
	/// as those found on the querystring.
	/// </summary>
	public class QueryStringParameterToHttpHeaderMiddleware
    {
        #region Private Members
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string _queryStringParameterName;
        private readonly string _headerName;
        private readonly Func<string, string>? _valueTransformer;
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStringParameterToHttpHeaderMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="queryStringParameterName">Name of the query string parameter.</param>
		/// <param name="headerName">Name of the header.</param>
		/// <param name="valueTransformer">The value transformer.</param>
		public QueryStringParameterToHttpHeaderMiddleware(
			RequestDelegate next,
			ILogger<QueryStringParameterToHttpHeaderMiddleware> logger,
			string queryStringParameterName,
			string headerName,
			Func<string, string>? valueTransformer = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(queryStringParameterName, nameof(queryStringParameterName));
            Guard.ArgumentNotNullOrWhiteSpace(headerName, nameof(headerName));

            _next = next;
            _logger = logger;
            _queryStringParameterName = queryStringParameterName;
            _headerName = headerName;
            _valueTransformer = valueTransformer;
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
                StringValues values = context.Request.Query[_queryStringParameterName];

                if (!StringValues.IsNullOrEmpty(values))
                {
                    string value = values.FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var headers = context.Request.GetTypedHeaders();

                        if (_valueTransformer != null)
                        {
                            string newValue = _valueTransformer(value);

                            if (string.IsNullOrWhiteSpace(newValue))
                                _logger.LogWarning($"The {nameof(QueryStringParameterToHttpHeaderMiddleware)} executed a value transformer which converted '{value}' to an empty string.");

                            value = newValue;
                        }

                        headers.Set(_headerName, value);
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
}