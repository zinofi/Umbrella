using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Interfaces;
using Umbrella.Utilities.FriendlyUrl;
using Umbrella.Utilities.Mime;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUtilities(this IServiceCollection services)
        {
            services.AddTransient<IEmailBuilder, EmailBuilder>();
            services.AddSingleton<IFriendlyUrlGenerator, FriendlyUrlGenerator>();
            services.AddSingleton<IMimeTypeUtility, MimeTypeUtility>();

            //Encryption
            services.AddSingleton<ICertificateUtility, CertificateUtility>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            return services;
        }
    }
}