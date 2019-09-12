using System;
using Umbrella.DataAccess.MultiTenant.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.DataAccess.MultiTenant"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.DataAccess.MultiTenant"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDataAccessMultiTenant(this IServiceCollection services)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddScoped(typeof(DbAppTenantSessionContext<>));

			return services;
		}
	}
}