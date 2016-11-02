using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Umbrella.AspNetCore.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
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