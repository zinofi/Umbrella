using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.ActiveDirectory
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaActiveDirectory(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationTokenUtility, AuthenticationTokenUtility>();

            return services;
        }
    }
}