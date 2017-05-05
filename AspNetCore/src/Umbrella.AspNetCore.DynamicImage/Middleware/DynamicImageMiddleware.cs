using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Umbrella.AspNetCore.DynamicImage.Middleware.Options;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.WebUtilities.Http;

namespace Umbrella.AspNetCore.DynamicImage.Middleware
{
    public class DynamicImageMiddleware
    {
        #region Private Members
        private readonly RequestDelegate m_Next;
        private readonly ILogger m_Logger;
        private readonly DynamicImageConfigurationOptions m_DynamicImageConfigurationOptions;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IDynamicImageResizer m_DynamicImageResizer;
        private readonly IHostingEnvironment m_HostingEnvironment;
        private readonly IHttpHeaderValueUtility m_HeaderValueUtility;
        private readonly DynamicImageMiddlewareOptions m_MiddlewareOptions = new DynamicImageMiddlewareOptions();
        #endregion

        #region Constructors
        public DynamicImageMiddleware(RequestDelegate next,
            ILogger<DynamicImageMiddleware> logger,
            IOptions<DynamicImageConfigurationOptions> configOptions,
            IDynamicImageUtility dynamicImageUtility,
            IDynamicImageResizer dynamicImageResizer,
            IHostingEnvironment hostingEnvironment,
            IHttpHeaderValueUtility headerValueUtility,
            Action<DynamicImageMiddlewareOptions> optionsBuilder)
        {
            m_Next = next;
            m_Logger = logger;
            m_DynamicImageConfigurationOptions = configOptions.Value;
            m_DynamicImageUtility = dynamicImageUtility;
            m_DynamicImageResizer = dynamicImageResizer;
            m_HostingEnvironment = hostingEnvironment;
            m_HeaderValueUtility = headerValueUtility;

            optionsBuilder?.Invoke(m_MiddlewareOptions);

            Guard.ArgumentNotNull(m_MiddlewareOptions.SourceFileProvider, nameof(m_MiddlewareOptions.SourceFileProvider));
            Guard.ArgumentNotNullOrWhiteSpace(m_MiddlewareOptions.DynamicImagePathPrefix, nameof(m_MiddlewareOptions.DynamicImagePathPrefix));

            //TODO: Add in validation to protected against multiple instances of the middleware being registered using
            //the same path prefix
        }
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var result = m_DynamicImageUtility.TryParseUrl(m_MiddlewareOptions.DynamicImagePathPrefix, context.Request.Path.Value);

                if (result.Status == DynamicImageParseUrlResult.Skip)
                {
                    await m_Next.Invoke(context);
                    return;
                }

                if (result.Status == DynamicImageParseUrlResult.Invalid || !m_DynamicImageUtility.ImageOptionsValid(result.ImageOptions, m_DynamicImageConfigurationOptions))
                {
                    SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                    return;
                }
                
                DynamicImageItem image = await m_DynamicImageResizer.GenerateImageAsync(m_MiddlewareOptions.SourceFileProvider, result.ImageOptions, context.RequestAborted);

                if(image == null)
                {
                    SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                    return;
                }

                //Check the cache headers
                if (context.Request.IfModifiedSinceHeaderMatched(image.LastModified))
                {
                    SetResponseStatusCode(context.Response, HttpStatusCode.NotModified);
                    return;
                }

                string eTagValue = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

                if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
                {
                    SetResponseStatusCode(context.Response, HttpStatusCode.NotModified);
                    return;
                }

                byte[] content = await image.GetContentAsync();

                if (content?.Length > 0)
                {
                    AppendResponseHeaders(context.Response, image);

                    await context.Response.Body.WriteAsync(content, 0, content.Length);
                    return;
                }
                else
                {
                    SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                    return;
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: false))
            {
                SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                return;
            }
        }
        #endregion

        #region Private Methods
        private void AppendResponseHeaders(HttpResponse response, DynamicImageItem image)
        {
            response.Headers["Content-Type"] = "image/" + image.ImageOptions.Format.ToString().ToLower();
            response.Headers["Last-Modified"] = m_HeaderValueUtility.CreateLastModifiedHeaderValue(image.LastModified);
            response.Headers["ETag"] = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);
        }

        private void SetResponseStatusCode(HttpResponse response, HttpStatusCode statusCode)
        {
            response.Clear();
            response.StatusCode = (int)statusCode;
        }
        #endregion
    }
}