using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Umbrella.Extensions.Logging.Azure.Management;

// TODO: Change this namespace to Microsoft.Extensions.DependencyInjection
[assembly:InternalsVisibleTo("Umbrella.Extensions.Logging.Azure.Test")]
namespace Umbrella.Extensions.Logging.Azure
{
    public static class IServiceCollectionExtensions
    {
		// TODO: Need to enable an optionsBuilder to be provided here to configure the options.
        public static IServiceCollection AddUmbrellaLoggingAzureStorage(this IServiceCollection services)
        {
            services.AddSingleton<IAzureTableStorageLogManager, AzureTableStorageLogManager>();

            return services;
        }
    }
}