using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;

namespace Umbrella.AspNetCore.WebUtilities
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ValidateModelStateAttribute>();

            return services;
        }
    }
}