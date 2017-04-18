using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching.AzureStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageBlobStorageCache(this IServiceCollection services, DynamicImageBlobStorageCacheOptions cacheOptions)
        {
            services.AddSingleton(cacheOptions);
            services.ReplaceSingleton<IDynamicImageCache, DynamicImageBlobStorageCache>();

            return services;
        }
    }
}