using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.AspNetCore.DynamicImage.Middleware;
using Umbrella.AspNetCore.DynamicImage.Middleware.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUmbrellaDynamicImage(this IApplicationBuilder builder, Action<DynamicImageMiddlewareOptions> config) => builder.UseMiddleware<DynamicImageMiddleware>(config);
    }
}