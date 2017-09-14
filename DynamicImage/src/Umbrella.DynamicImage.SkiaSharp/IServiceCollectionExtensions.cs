using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.SkiaSharp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageSkiaSharp(this IServiceCollection services)
        {
            services.AddSingleton<DynamicImageResizerBase, DynamicImageResizer>();

            return services;
        }
    }
}