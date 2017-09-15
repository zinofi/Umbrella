using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAzureBlobStorageFileProvider(this IServiceCollection services, UmbrellaAzureBlobStorageFileProviderOptions options)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(options, nameof(options));

            services.AddSingleton(options);
            services.AddSingleton<IUmbrellaFileProvider, UmbrellaAzureBlobStorageFileProvider>();
            services.AddSingleton<IUmbrellaAzureBlobStorageFileProvider, UmbrellaAzureBlobStorageFileProvider>();

            return services;
        }
    }
}