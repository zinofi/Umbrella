using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImage(this IServiceCollection services, DynamicImageCacheOptions cacheOptions)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(cacheOptions, nameof(cacheOptions));

            services.AddSingleton(cacheOptions);
            services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();
            services.AddSingleton<IDynamicImageCache, DynamicImageNoCache>();

            return services;
        }

        public static IServiceCollection AddUmbrellaDynamicImageMemoryCache(this IServiceCollection services, DynamicImageMemoryCacheOptions cacheOptions)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(cacheOptions, nameof(cacheOptions));

            services.AddSingleton(cacheOptions);
            services.AddSingleton<IDynamicImageCache, DynamicImageMemoryCache>();

            return services;
        }

        public static IServiceCollection AddUmbrellaDynamicImageDiskCache(this IServiceCollection services, DynamicImageDiskCacheOptions cacheOptions)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(cacheOptions, nameof(cacheOptions));

            services.AddSingleton(cacheOptions);
            services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();

            return services;
        }
    }
}