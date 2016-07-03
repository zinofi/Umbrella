using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
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