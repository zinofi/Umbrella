using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.Utilities.Configuration;
using Umbrella.Utilities.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaLegacyUtilitiesAppSettings(this IServiceCollection services)
        {
            services.AddSingleton<IReadOnlyAppSettingsSource, AppConfigReadOnlyAppSettingsSource>();

            return services;
        }
    }
}