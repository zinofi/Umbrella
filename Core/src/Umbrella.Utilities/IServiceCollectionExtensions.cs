using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.DependencyInjection;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.FriendlyUrl;
using Umbrella.Utilities.Mime;

[assembly: InternalsVisibleTo("Umbrella.Utilities.Benchmark")]

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUtilities(this IServiceCollection services)
        {
            Guard.ArgumentNotNull(services, nameof(services));

            services.AddTransient(typeof(Lazy<>), typeof(LazyProxy<>));
            services.AddTransient<IEmailBuilder, EmailBuilder>();
            services.AddSingleton<IFriendlyUrlGenerator, FriendlyUrlGenerator>();
            services.AddSingleton<IMimeTypeUtility, MimeTypeUtility>();
            services.AddSingleton<ICacheKeyUtility, CacheKeyUtility>();
            services.AddSingleton<ICertificateUtility, CertificateUtility>();
            services.AddSingleton<ISecureStringGenerator, SecureStringGenerator>();
            services.AddSingleton<IMultiCache, MultiCache>();
            services.AddSingleton<INonceGenerator, NonceGenerator>();

            // Default Options - These can be replaced by calls to the Configure* methods below.
            services.AddSingleton(serviceProvider =>
            {
                var cacheKeyUtility = serviceProvider.GetService<ICacheKeyUtility>();

                return new MultiCacheOptions
                {
                    CacheKeyBuilder = (type, key) => cacheKeyUtility.Create(type, key)
                };
            });

            return services;
        }

        public static IServiceCollection ConfigureMultiCacheOptions(this IServiceCollection services, Action<IServiceProvider, MultiCacheOptions> optionsBuilder)
        {
            Guard.ArgumentNotNull(services, nameof(services));

            services.ReplaceSingleton(serviceProvider =>
            {
                var cacheKeyUtility = serviceProvider.GetService<ICacheKeyUtility>();

                var options = new MultiCacheOptions
                {
                    CacheKeyBuilder = (type, key) => cacheKeyUtility.Create(type, key)
                };

                optionsBuilder?.Invoke(serviceProvider, options);

                return options;
            });

            return services;
        }
    }
}