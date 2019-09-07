using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    // TODO: Alter all service registration methods that accept an options instance to
    // use the Action<TOptions> builder pattern everywhere to make it all consistent.
    // There are quite a few inconsistencies. Save until v3 though as will be a breaking change.
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Used to register the Dynamic Image Azure Blob Storage Cache services with the DI container for the application. Any existing registrations for <see cref="IDynamicImageCache"/> will be replaced with <see cref="DynamicImageAzureBlobStorageCache"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for the application.</param>
        /// <param name="cacheOptions">The <see cref="DynamicImageAzureBlobStorageCacheOptions"/> cache options.</param>
        /// <returns>The services collection for the application.</returns>
        public static IServiceCollection AddUmbrellaDynamicImageAzureBlobStorageCache(this IServiceCollection services, DynamicImageAzureBlobStorageCacheOptions cacheOptions)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(cacheOptions, nameof(cacheOptions));

            services.AddSingleton(cacheOptions);
            services.AddSingleton<IDynamicImageCache, DynamicImageAzureBlobStorageCache>();

            return services;
        }
    }
}