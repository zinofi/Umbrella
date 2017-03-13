using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.MultiTenant;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDataAccessMultiTenant(this IServiceCollection services)
        {
            services.AddScoped(typeof(DbAppTenantSessionContext<>));

            return services;
        }
    }
}