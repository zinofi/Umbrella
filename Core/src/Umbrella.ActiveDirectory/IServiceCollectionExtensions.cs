using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.ActiveDirectory;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.ActiveDirectory"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
    {
		/// <summary>
		/// Adds the services for the 
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddUmbrellaActiveDirectory(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationTokenUtility, AuthenticationTokenUtility>();

            return services;
        }
    }
}