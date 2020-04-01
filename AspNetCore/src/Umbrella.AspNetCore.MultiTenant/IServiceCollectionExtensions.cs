using System;
using Umbrella.AspNetCore.MultiTenant.Middleware.Options;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.AspNetCore.MultiTenant"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.MultiTenant"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="multiTenantSessionContextMiddlewareOptionsBuilder">The optional <see cref="MultiTenantSessionContextMiddlewareOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaAspNetCoreWebUtilitiesMultiTenant(
			this IServiceCollection services,
			Action<IServiceProvider, MultiTenantSessionContextMiddlewareOptions>? multiTenantSessionContextMiddlewareOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			// Options
			services.ConfigureUmbrellaOptions(multiTenantSessionContextMiddlewareOptionsBuilder);

			return services;
		}
	}
}