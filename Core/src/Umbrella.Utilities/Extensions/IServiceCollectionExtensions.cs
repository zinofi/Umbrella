using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions2
    {
        public static IServiceCollection ReplaceTransient<TService>(this IServiceCollection services)
            where TService : class
            => services.Remove<TService>().AddTransient<TService>();

        public static IServiceCollection ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Remove<TService>().AddTransient<TService, TImplementation>();

        public static IServiceCollection ReplaceScoped<TService>(this IServiceCollection services)
            where TService : class
            => services.Remove<TService>().AddScoped<TService>();

        public static IServiceCollection ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Remove<TService>().AddScoped<TService, TImplementation>();

        public static IServiceCollection ReplaceSingleton<TService>(this IServiceCollection services)
            where TService : class
            => services.Remove<TService>().AddSingleton<TService>();

        public static IServiceCollection ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Remove<TService>().AddSingleton<TService, TImplementation>();

        public static IServiceCollection Remove<TService>(this IServiceCollection services)
        {
            ServiceDescriptor serviceToRemove = services.SingleOrDefault(x => x.ServiceType == typeof(TService));

            if (serviceToRemove != null)
                services.Remove(serviceToRemove);

            return services;
        }
    }
}