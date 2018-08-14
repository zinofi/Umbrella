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
            => AddUmbrellaAzureBlobStorageFileProvider<UmbrellaAzureBlobStorageFileProvider>(services, options);

        public static IServiceCollection AddUmbrellaAzureBlobStorageFileProvider<TFileProvider>(this IServiceCollection services, UmbrellaAzureBlobStorageFileProviderOptions options)
            where TFileProvider : class, IUmbrellaAzureBlobStorageFileProvider
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(options, nameof(options));

            services.AddSingleton(options);
            services.AddSingleton<IUmbrellaAzureBlobStorageFileProvider, TFileProvider>();
            services.AddSingleton<IUmbrellaFileProvider>(x => x.GetService<IUmbrellaAzureBlobStorageFileProvider>());

            return services;
        }
    }
}