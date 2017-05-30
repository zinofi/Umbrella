using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching.AzureStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Used to register the Dynamic Image Azure Blob Storage Cache services with the DI container for the application. Any existing registrations for <see cref="IDynamicImageCache"/> will be replaced with <see cref="DynamicImageAzureBlobStorageCache"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for the application.</param>
        /// <param name="cacheOptions">The <see cref="DynamicImageAzureBlobStorageCacheOptions"/> cache options. A default implementation will be registered if this is not provided.</param>
        /// <returns>The services collection for the application.</returns>
        public static IServiceCollection AddUmbrellaDynamicImageAzureBlobStorageCache(this IServiceCollection services, DynamicImageAzureBlobStorageCacheOptions cacheOptions = null)
        {
            if (cacheOptions == null)
                cacheOptions = new DynamicImageAzureBlobStorageCacheOptions();

            services.AddSingleton(cacheOptions);
            services.ReplaceSingleton<IDynamicImageCache, DynamicImageAzureBlobStorageCache>();

            return services;
        }
    }
}