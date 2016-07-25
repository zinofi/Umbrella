using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.MultiTenant
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBARcossBLManagement(this IServiceCollection services, string managementConnectionString)
        {
            services.AddScoped(typeof(DbAppTenantSessionContext<>));

            return services;
        }
    }
}
