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

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
    public class DynamicImageMiddleware : OwinMiddleware
    {
        #region Private Members
        private readonly ILogger Log;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IDynamicImageResizer m_DynamicImageResizer;
        private readonly string m_DynamicImagePathPrefix;
        private readonly Lazy<DynamicImageConfigurationOptions> m_ConfigurationOptions = new Lazy<DynamicImageConfigurationOptions>(LoadConfigurationOptions);
        private readonly Func<string, Task<(byte[], DateTime)>> m_SourceImageResolver;
        #endregion

        #region Private Properties
        private DynamicImageConfigurationOptions ConfigurationOptions => m_ConfigurationOptions.Value;
        #endregion

        #region Constructors
        public DynamicImageMiddleware(OwinMiddleware next,
            ILogger<DynamicImageMiddleware> logger,
            IDynamicImageUtility dynamicImageUtility,
            IDynamicImageResizer dynamicImageResizer,
            string dynamicImagePathPrefix,
            Func<string, Task<(byte[], DateTime)>> sourceImageResolver)
            : base(next)
        {
            Log = logger;
            m_DynamicImageUtility = dynamicImageUtility;
            m_DynamicImageResizer = dynamicImageResizer;
            m_DynamicImagePathPrefix = dynamicImagePathPrefix;
            m_SourceImageResolver = sourceImageResolver;
        }
        #endregion

        #region Overridden Methods
        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                var result = m_DynamicImageUtility.TryParseUrl(m_DynamicImagePathPrefix, context.Request.Path.Value);

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

                DynamicImageItem image = await m_DynamicImageResizer.GenerateImageAsync(m_SourceImageResolver, result.ImageOptions);

                //If the image in the cache hasn't been modified
                string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(ifModifiedSince))
                {
                    DateTime lastModified = DateTime.Parse(ifModifiedSince);

                    if (lastModified == image.LastModified)
                    {
                        await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                        return;
                    }
                }

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
            response.Headers.Append("Last-Modified", dynamicImage.LastModified.ToUniversalTime().ToString("r"));
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