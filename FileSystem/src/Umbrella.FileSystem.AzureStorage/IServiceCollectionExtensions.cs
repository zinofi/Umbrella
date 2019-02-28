using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    // TODO: Add an additional method that allows a custom provider to be registered
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

            // TODO: Remove this so that the consuming app has to make an explicit choice about the file provider used as default - pass in a param to say "bindToDefault" or something.
            services.AddSingleton<IUmbrellaFileProvider>(x => x.GetService<IUmbrellaAzureBlobStorageFileProvider>());

            return services;
        }
    }
}