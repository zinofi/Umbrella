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
        public static IServiceCollection AddUmbrellaDynamicImage(this IServiceCollection services,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageMemoryCacheOptions memoryCacheOptions = null,
            DynamicImageDiskCacheOptions diskCacheOptions = null)
        {
            services.AddSingleton(cacheOptions);

            if (memoryCacheOptions == null)
                memoryCacheOptions = new DynamicImageMemoryCacheOptions();

            if (diskCacheOptions != null)
                diskCacheOptions = new DynamicImageDiskCacheOptions();

            services.AddSingleton(memoryCacheOptions);
            services.AddSingleton(diskCacheOptions);
            services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();
            services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();

            return services;
        }
    }
}