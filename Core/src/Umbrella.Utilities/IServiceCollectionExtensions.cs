using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUtilities(this IServiceCollection services)
        {
            //Email
            services.AddTransient<IEmailBuilder, EmailBuilder>();

            //Encryption
#if NET46
            services.AddSingleton<ICertificateUtility, CertificateUtility>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
#endif

            return services;
        }
    }
}