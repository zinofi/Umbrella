using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;
using Umbrella.Utilities.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ValidateModelStateAttribute>();
            services.AddSingleton<IUmbrellaHostingEnvironment, UmbrellaHostingEnvironment>();

            return services;
        }
    }
}