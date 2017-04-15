using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImage(this IServiceCollection services, DynamicImageCacheOptions options = null)
        {
            if (options != null)
                services.AddSingleton(options);

            services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();
            services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();

            return services;
        }
    }
}