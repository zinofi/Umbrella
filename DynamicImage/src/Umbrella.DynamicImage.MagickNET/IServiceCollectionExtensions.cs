using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.MagickNET;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageMagickNET(this IServiceCollection services)
        {
            services.AddSingleton<DynamicImageResizerBase, DynamicImageResizer>();

            return services;
        }
    }
}