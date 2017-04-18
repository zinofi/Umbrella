using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageBlobStorageCache(this IServiceCollection services)
        {
            services.ReplaceSingleton<IDynamicImageCache, DynamicImageBlobStorageCache>();

            return services;
        }
    }
}