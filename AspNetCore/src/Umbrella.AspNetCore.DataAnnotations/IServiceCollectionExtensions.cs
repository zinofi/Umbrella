using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace Umbrella.AspNetCore.DataAnnotations
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreDataAnnotations(this IServiceCollection services)
        {
            ServiceDescriptor existingService = services.SingleOrDefault(x => x.ServiceType == typeof(IValidationAttributeAdapterProvider));

            if (existingService != null)
                services.Remove(existingService);

            services.AddSingleton<IValidationAttributeAdapterProvider, UmbrellaValidationAttributeAdapterProvider>();

            return services;
        }
    }
}