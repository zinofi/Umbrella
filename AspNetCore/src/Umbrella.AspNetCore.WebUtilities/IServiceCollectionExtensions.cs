using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;
using Umbrella.Utilities.Hosting;
using Umbrella.WebUtilities.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ValidateModelStateAttribute>();

            //Add the hosting environment as a singleton and then ensure the same instance is bound to both interfaces
            services.AddSingleton<UmbrellaWebHostingEnvironment>();
            services.AddSingleton<IUmbrellaHostingEnvironment>(x => x.GetService<UmbrellaWebHostingEnvironment>());
            services.AddSingleton<IUmbrellaWebHostingEnvironment>(x => x.GetService<UmbrellaWebHostingEnvironment>());

            return services;
        }
    }
}