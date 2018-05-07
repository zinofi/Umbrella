using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Middleware.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.WebUtilities.Middleware
{
    //TODO: Revisit this - needs only apply to requests that contain the "X-Requested-With" header with a value of "XMLHttpRequest"
    //TODO: Implement a corresponding version for the legacy stack
    public class InternetExplorerCacheHeaderMiddleware
    {
        #region Private Members
        private readonly RequestDelegate m_Next;
        private readonly ILogger m_Logger;
        private readonly InternetExplorerCacheHeaderOptions m_Options = new InternetExplorerCacheHeaderOptions();
        #endregion

        #region Constructors
        public InternetExplorerCacheHeaderMiddleware(RequestDelegate next,
            ILogger<InternetExplorerCacheHeaderMiddleware> logger,
            Action<InternetExplorerCacheHeaderOptions> optionsBuilder)
        {
            m_Next = next;
            m_Logger = logger;

            optionsBuilder?.Invoke(m_Options);

            Guard.ArgumentNotNull(m_Options.ContentTypes, $"{nameof(InternetExplorerCacheHeaderOptions)}.{nameof(m_Options.ContentTypes)}");
            Guard.ArgumentNotNull(m_Options.Methods, $"{nameof(InternetExplorerCacheHeaderOptions)}.{nameof(m_Options.Methods)}");
            Guard.ArgumentNotNull(m_Options.UserAgentKeywords, $"{nameof(InternetExplorerCacheHeaderOptions)}.{nameof(m_Options.UserAgentKeywords)}");
        }
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
        {
            try
            {
                string method = context.Request.Method;
                string userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

                bool addHeaders = !string.IsNullOrWhiteSpace(userAgent)
                    && (m_Options.UserAgentKeywords.Count == 0 || m_Options.UserAgentKeywords.Any(x => userAgent.Contains(x)))
                    && (m_Options.Methods.Count == 0 || m_Options.Methods.Any(x => method.Contains(x)));

                if (addHeaders)
                {
                    context.Response.OnStarting(state =>
                    {
                        HttpResponse response = (HttpResponse)state;

                        string contentType = response.ContentType;
                        
                        //Apply the headers when no response content type - this may be the case when a 204 code is sent for a GET request that issues no content
                        //If no content types have been specified in the options apply the headers to all response for the configured HTTP methods
                        //If content types have been specified to filter on then only apply the headers for those responses
                        if (string.IsNullOrWhiteSpace(contentType) || m_Options.ContentTypes.Count == 0 || m_Options.ContentTypes.Any(x => contentType.Contains(x)))
                        {
                            //Set standard HTTP/1.0 no-cache header (no-store, no-cache, must-revalidate)
                            //Set IE extended HTTP/1.1 no-cache headers (post-check, pre-check)
                            response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

                            //Set standard HTTP/1.0 no-cache header.
                            response.Headers.Add("Pragma", "no-cache");

                            //Set the Expires header.
                            response.Headers.Add("Expires", "0");
                        }

                        return Task.CompletedTask;

                    }, context.Response);
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