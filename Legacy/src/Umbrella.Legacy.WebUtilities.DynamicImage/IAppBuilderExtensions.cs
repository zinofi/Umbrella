using Microsoft.Extensions.Logging;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.WebUtilities.Http;

namespace Owin
{
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseUmbrellaDynamicImage(this IAppBuilder builder,
            ILogger<DynamicImageMiddleware> logger,
            IDynamicImageUtility dynamicImageUtility,
            IDynamicImageResizer dynamicImageResizer,
            IHttpHeaderValueUtility headerValueUtility,
            Action<DynamicImageMiddlewareOptions> optionsBuilder = null)
            => builder.Use<DynamicImageMiddleware>(logger, dynamicImageUtility, dynamicImageResizer, headerValueUtility, optionsBuilder);
    }
}