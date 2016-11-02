using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.ActiveDirectory;

namespace Microsoft.Extensions.DependencyInjection
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