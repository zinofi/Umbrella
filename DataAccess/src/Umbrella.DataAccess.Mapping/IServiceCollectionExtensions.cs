using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Mapping
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDataAccessMapping(this IServiceCollection services)
        {
            services.AddSingleton<IMappingUtility, MappingUtility>();

            return services;
        }
    }
}