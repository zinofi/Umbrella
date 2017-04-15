using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Umbrella.AspNetCore.DataAnnotations;
using Umbrella.Utilities.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreDataAnnotations(this IServiceCollection services)
        {
            services.ReplaceSingleton<IValidationAttributeAdapterProvider, UmbrellaValidationAttributeAdapterProvider>();

            return services;
        }
    }
}