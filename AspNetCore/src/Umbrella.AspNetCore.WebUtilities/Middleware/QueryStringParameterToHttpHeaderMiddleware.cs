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
    public class QueryStringParameterToHttpHeaderMiddleware
    {
        #region Private Members
        private readonly RequestDelegate m_Next;
        private readonly ILogger m_Logger;
        private readonly string m_QueryStringParameterName;
        private readonly string m_HeaderName;
        private readonly Func<string, string> m_ValueTransformer;
        #endregion

        #region Constructors
        public QueryStringParameterToHttpHeaderMiddleware(RequestDelegate next, ILogger<QueryStringParameterToHttpHeaderMiddleware> logger, string queryStringParameterName, string headerName, Func<string, string> valueTransformer = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(queryStringParameterName, nameof(queryStringParameterName));
            Guard.ArgumentNotNullOrWhiteSpace(headerName, nameof(headerName));

            m_Next = next;
            m_Logger = logger;
            m_QueryStringParameterName = queryStringParameterName;
            m_HeaderName = headerName;
            m_ValueTransformer = valueTransformer;
        }
        #endregion

        #region Middleware Members
        public async Task Invoke(HttpContext context)
        {
            try
            {
                StringValues values = context.Request.Query[m_QueryStringParameterName];

                if (!StringValues.IsNullOrEmpty(values))
                {
                    string value = values.FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var headers = context.Request.GetTypedHeaders();

                        if (m_ValueTransformer != null)
                        {
                            string newValue = m_ValueTransformer(value);

                            if (string.IsNullOrWhiteSpace(newValue))
                                m_Logger.LogWarning($"The {nameof(QueryStringParameterToHttpHeaderMiddleware)} executed a value transformer which converted '{value}' to an empty string.");

                            value = newValue;
                        }

                        headers.Set(m_HeaderName, value);
                    }
                }

                await m_Next.Invoke(context);
            }
            catch (Exception exc) when (m_Logger.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}