using System;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Umbrella.AspNetCore.WebUtilities.Security;
using Umbrella.AspNetCore.WebUtilities.Security.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.WebUtilities.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.AspNetCore.WebUtilities"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="apiIntegrationCookieAuthenticationEventsOptionsBuilder">The optional <see cref="ApiIntegrationCookieAuthenticationEventsOptions"/> builder.</param>
		/// <param name="umbrellaClaimsUserAccessorOptionsOptionsBuilder">The optional <see cref="UmbrellaClaimsUserAccessorOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities(
			this IServiceCollection services,
			Action<IServiceProvider, ApiIntegrationCookieAuthenticationEventsOptions>? apiIntegrationCookieAuthenticationEventsOptionsBuilder = null,
			Action<IServiceProvider, UmbrellaClaimsUserAccessorOptions>? umbrellaClaimsUserAccessorOptionsOptionsBuilder = null)
			=> services.AddUmbrellaAspNetCoreWebUtilities<UmbrellaWebHostingEnvironment>(
				apiIntegrationCookieAuthenticationEventsOptionsBuilder,
				umbrellaClaimsUserAccessorOptionsOptionsBuilder);

		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TUmbrellaWebHostingEnvironment">
		/// The concrete implementation of <see cref="IUmbrellaWebHostingEnvironment"/> to register. This allows consuming applications to override the default implementation and allow it to be
		/// resolved from the container correctly for both the <see cref="IUmbrellaHostingEnvironment"/> and <see cref="IUmbrellaWebHostingEnvironment"/> interfaces.
		/// </typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="apiIntegrationCookieAuthenticationEventsOptionsBuilder">The optional <see cref="ApiIntegrationCookieAuthenticationEventsOptions"/> builder.</param>
		/// <param name="umbrellaClaimsUserAccessorOptionsOptionsBuilder">The optional <see cref="UmbrellaClaimsUserAccessorOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities<TUmbrellaWebHostingEnvironment>(
			this IServiceCollection services,
			Action<IServiceProvider, ApiIntegrationCookieAuthenticationEventsOptions>? apiIntegrationCookieAuthenticationEventsOptionsBuilder = null,
			Action<IServiceProvider, UmbrellaClaimsUserAccessorOptions>? umbrellaClaimsUserAccessorOptionsOptionsBuilder = null)
			where TUmbrellaWebHostingEnvironment : class, IUmbrellaWebHostingEnvironment
		{
			Guard.ArgumentNotNull(services, nameof(services));

			// Add the hosting environment as a singleton and then ensure the same instance is bound to both interfaces
			services.AddSingleton<TUmbrellaWebHostingEnvironment>();
			services.ReplaceSingleton<IUmbrellaHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());
			services.ReplaceSingleton<IUmbrellaWebHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());

			services.AddSingleton<ApiIntegrationCookieAuthenticationEvents>();

			services.ConfigureUmbrellaOptions(apiIntegrationCookieAuthenticationEventsOptionsBuilder);
			services.ConfigureUmbrellaOptions(umbrellaClaimsUserAccessorOptionsOptionsBuilder);

			return services;
		}
	}
}