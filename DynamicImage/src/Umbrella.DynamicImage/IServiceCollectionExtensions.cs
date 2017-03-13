using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage;
using Umbrella.DynamicImage.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImage(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();
            services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();

            return services;
        }
    }
}