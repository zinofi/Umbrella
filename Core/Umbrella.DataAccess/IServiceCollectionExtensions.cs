using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess;
using Umbrella.DataAccess.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDataAccess(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IUserAuditDataFactory<>), typeof(NoopUserAuditDataFactory<>));
            services.AddSingleton<IDataAccessLookupNormalizer, DataAccessUpperInvariantLookupNormalizer>();

            return services;
        }
    }
}