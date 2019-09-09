using Umbrella.DataAccess.MultiTenant.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// This class contains extension methods used to add the Multi-Tenant Data Access services with the specified
	/// <see cref="IServiceCollection"/>.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the Umbrella MultiTenant Data Access services to the specified service colletion.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
		/// <returns>The same <see cref="IServiceCollection"/> as was passed in but which now has the MultiTenant services added to it.</returns>
		public static IServiceCollection AddUmbrellaDataAccessMultiTenant(this IServiceCollection services)
		{
			services.AddScoped(typeof(DbAppTenantSessionContext<>));

			return services;
		}
	}
}
