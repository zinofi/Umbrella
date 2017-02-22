using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.AspNetCore.WebUtilities.Middleware.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUmbrellaDynamicImage(this IApplicationBuilder builder, Action<DynamicImageMiddlewareOptions> config) => builder.UseMiddleware<DynamicImageMiddleware>(config);
    }
}