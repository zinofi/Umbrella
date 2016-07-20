using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Interfaces;

namespace Umbrella.Utilities
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUtilities(this IServiceCollection services)
        {
            //Email
            services.AddTransient<IEmailBuilder, EmailBuilder>();
            services.AddTransient<IEmailHelper, EmailHelper>();

            //Encryption
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            return services;
        }
    }
}