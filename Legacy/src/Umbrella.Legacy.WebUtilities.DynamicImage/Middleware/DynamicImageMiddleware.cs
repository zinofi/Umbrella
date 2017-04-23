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

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
    public class DynamicImageMiddleware : OwinMiddleware
    {
        #region Private Members
        private readonly ILogger Log;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IDynamicImageResizer m_DynamicImageResizer;
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
            Action<DynamicImageMiddlewareOptions> optionsBuilder)
            : base(next)
        {
            Log = logger;
            m_DynamicImageUtility = dynamicImageUtility;
            m_DynamicImageResizer = dynamicImageResizer;

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
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }

                DynamicImageItem image = await m_DynamicImageResizer.GenerateImageAsync(m_MiddlewareOptions.SourceFileProvider, result.ImageOptions, context.Request.CallCancelled);

                if(image == null)
                {
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }

                //If the image in the cache hasn't been modified
                string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(ifModifiedSince))
                {
                    DateTime lastModified = DateTime.Parse(ifModifiedSince).ToUniversalTime();

                    if (lastModified == image.LastModified.UtcDateTime)
                    {
                        await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                        return;
                    }
                }

                //TODO: Check the If-None-Match header as well if the If-Modified-Since header is missing

                byte[] content = await image.GetContentAsync();

                if (content?.Length > 0)
                {
                    AppendResponseHeaders(context.Response, image);

                    await context.Response.WriteAsync(content);
                    return;
                }
                else
                {
                    await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                    return;
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
            {
                await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                return;
            }
        }
        #endregion

        #region Private Methods
        private void AppendResponseHeaders(IOwinResponse response, DynamicImageItem dynamicImage)
        {
            response.Headers.Append("Content-Type", "image/" + dynamicImage.ImageOptions.Format.ToString().ToLower());
            response.Headers.Append("Last-Modified", dynamicImage.LastModified.UtcDateTime.ToString("r"));

            //TODO: ASP.NET Core seems to require this to be present on responses otherwise it won't
            //read the If-Modified-Since request header value when the ETag is not sent back in the If-None-Match header.
            //Not sure if this is Kestrel or IIS that is exhibiting this behaviour. Check if this is a problem in classic
            //ASP.NET or not. Not exactly a big deal to leave this here though.
            long eTagHash = dynamicImage.LastModified.UtcDateTime.ToFileTimeUtc() ^ dynamicImage.Length;
            string eTagValue = Convert.ToString(eTagHash, 16);

            response.Headers.Append("ETag", $"\"{eTagValue}\"");
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