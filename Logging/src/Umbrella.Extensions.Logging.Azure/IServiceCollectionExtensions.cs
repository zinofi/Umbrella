using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Umbrella.Extensions.Logging.Azure.Management;

[assembly:InternalsVisibleTo("Umbrella.Extensions.Logging.Azure.Test")]
namespace Umbrella.Extensions.Logging.Azure
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaLoggingAzureStorage(this IServiceCollection services)
        {
            services.AddSingleton<IAzureTableStorageLogManager, AzureTableStorageLogManager>();

            return services;
        }
    }
}