using log4net;
using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Umbrella.Utilities.Extensions;
using Umbrella.Legacy.WebUtilities.Extensions;
using System.Net;
using System.Web.Configuration;
using Umbrella.Legacy.WebUtilities.DynamicImage.Configuration;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using System.IO;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Http;
using System.Threading;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
    public class DynamicImageMiddleware : OwinMiddleware
    {
        #region Private Members
        private readonly ILogger Log;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IDynamicImageResizer m_DynamicImageResizer;
        private readonly IHttpHeaderValueUtility m_HeaderValueUtility;
        private readonly Lazy<DynamicImageConfigurationOptions> m_ConfigurationOptions = new Lazy<DynamicImageConfigurationOptions>(LoadConfigurationOptions);
        private readonly DynamicImageMiddlewareOptions m_MiddlewareOptions = new DynamicImageMiddlewareOptions();
        #endregion

        #region Private Properties
        private DynamicImageConfigurationOptions ConfigurationOptions => m_ConfigurationOptions.Value;
        #endregion

        #region Constructors
        public DynamicImageMiddleware(OwinMiddleware next,
            ILogger<DynamicImageMiddleware> logger,
            IDynamicImageUtility dynamicImageUtility,
            IDynamicImageResizer dynamicImageResizer,
            IHttpHeaderValueUtility headerValueUtility,
            Action<DynamicImageMiddlewareOptions> optionsBuilder)
            : base(next)
        {
            Log = logger;
            m_DynamicImageUtility = dynamicImageUtility;
            m_DynamicImageResizer = dynamicImageResizer;
            m_HeaderValueUtility = headerValueUtility;

            optionsBuilder?.Invoke(m_MiddlewareOptions);

            Guard.ArgumentNotNull(m_MiddlewareOptions.SourceFileProvider, nameof(m_MiddlewareOptions.SourceFileProvider));
            Guard.ArgumentNotNullOrWhiteSpace(m_MiddlewareOptions.DynamicImagePathPrefix, nameof(m_MiddlewareOptions.DynamicImagePathPrefix));

            //TODO: Add in validation to protected against multiple instances of the middleware being registered using
            //the same path prefix
        }
        #endregion

        #region Overridden Methods
        public override async Task Invoke(IOwinContext context)
        {
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
            CancellationToken token = cts.Token;

            try
            {
                var result = m_DynamicImageUtility.TryParseUrl(m_MiddlewareOptions.DynamicImagePathPrefix, context.Request.Path.Value);

                if(result.Status == DynamicImageParseUrlResult.Skip)
                {
                    await Next.Invoke(context);
                    return;
                }

                if (result.Status == DynamicImageParseUrlResult.Invalid || !m_DynamicImageUtility.ImageOptionsValid(result.ImageOptions, ConfigurationOptions))
                {
                    cts.Cancel();
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }

                DynamicImageItem image = await m_DynamicImageResizer.GenerateImageAsync(m_MiddlewareOptions.SourceFileProvider, result.ImageOptions, token);

                if(image == null)
                {
                    cts.Cancel();
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }

                //Check the cache headers
                if(context.Request.IfModifiedSinceHeaderMatched(image.LastModified))
                {
                    cts.Cancel();
                    await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                    return;
                }

                string eTagValue = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

                if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
                {
                    cts.Cancel();
                    await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                    return;
                }

                byte[] content = await image.GetContentAsync(token);

                if (content?.Length > 0)
                {
                    AppendResponseHeaders(context.Response, image);

                    await context.Response.WriteAsync(content, token);
                    return;
                }
                else
                {
                    cts.Cancel();
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
            {
                cts.Cancel();
                await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                return;
            }
        }
        #endregion

        #region Private Methods
        private void AppendResponseHeaders(IOwinResponse response, DynamicImageItem image)
        {
            response.ContentType = "image/" + image.ImageOptions.Format.ToString().ToLower();
            response.Headers["Last-Modified"] = m_HeaderValueUtility.CreateLastModifiedHeaderValue(image.LastModified);
            response.ETag = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

            if (!string.IsNullOrWhiteSpace(m_MiddlewareOptions.CacheControlHeaderValue))
                response.Headers["Cache-Control"] = m_MiddlewareOptions.CacheControlHeaderValue.Trim().ToLowerInvariant();
        }

        private static DynamicImageConfigurationOptions LoadConfigurationOptions()
        {
            DynamicImageMappingsConfig mappingsConfig = new DynamicImageMappingsConfig(WebConfigurationManager.OpenWebConfiguration("~/web.config"));
            DynamicImageConfigurationOptions options = (DynamicImageConfigurationOptions)mappingsConfig;

            return options;
        }
        #endregion
    }
}