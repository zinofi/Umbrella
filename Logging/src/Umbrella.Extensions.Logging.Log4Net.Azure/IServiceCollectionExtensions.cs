using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Extensions.Logging.Log4Net.Azure.Management;
using Umbrella.Extensions.Logging.Log4Net.Azure.Management.Configuration;

namespace Umbrella.Extensions.Logging.Log4Net.Azure
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaLog4NetAzureStorage(this IServiceCollection services)
        {
            services.AddSingleton<IAzureTableStorageLogManager, AzureTableStorageLogManager>();

            return services;
        }
    }
}