using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
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
            services.AddTransient(typeof(Lazy<>), typeof(LazyProxy<>));
            services.AddTransient<IEmailBuilder, EmailBuilder>();
            services.AddSingleton<IFriendlyUrlGenerator, FriendlyUrlGenerator>();
            services.AddSingleton<IMimeTypeUtility, MimeTypeUtility>();
            services.AddSingleton<ICacheKeyUtility, CacheKeyUtility>();

            //Encryption
            services.AddSingleton<ICertificateUtility, CertificateUtility>();
            services.AddSingleton<ISecureStringGenerator, SecureStringGenerator>();

            return services;
        }
    }
}