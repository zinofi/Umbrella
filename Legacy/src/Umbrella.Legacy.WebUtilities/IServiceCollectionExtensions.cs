using System;
using System.Runtime.CompilerServices;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Legacy.WebUtilities.Middleware;
using Umbrella.Legacy.WebUtilities.Middleware.Options;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.WebUtilities.Hosting;

[assembly: InternalsVisibleTo("Umbrella.Legacy.WebUtilities.Test")]
[assembly: InternalsVisibleTo("Umbrella.Legacy.WebUtilities.Benchmark")]

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.Legacy.WebUtilities"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.Legacy.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="frontEndCompressionMiddlewareOptionsBuilder">The optional <see cref="FrontEndCompressionMiddlewareOptions"/> builder.</param>
		/// <param name="bundleUtilityOptionsBuilder">The optional <see cref="BundleUtilityOptions"/> builder.</param>
		/// <param name="webpackBundleUtilityOptionsBuilder">The optional <see cref="WebpackBundleUtilityOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaLegacyWebUtilities(
			this IServiceCollection services,
			Action<IServiceProvider, FrontEndCompressionMiddlewareOptions> frontEndCompressionMiddlewareOptionsBuilder = null,
			Action<IServiceProvider, BundleUtilityOptions> bundleUtilityOptionsBuilder = null,
			Action<IServiceProvider, WebpackBundleUtilityOptions> webpackBundleUtilityOptionsBuilder = null)
			=> services.AddUmbrellaLegacyWebUtilities<UmbrellaWebHostingEnvironment>(frontEndCompressionMiddlewareOptionsBuilder, bundleUtilityOptionsBuilder, webpackBundleUtilityOptionsBuilder);

		/// <summary>
		/// Adds the <see cref="Umbrella.Legacy.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TUmbrellaWebHostingEnvironment">
		/// The concrete implementation of <see cref="IUmbrellaWebHostingEnvironment"/> to register. This allows consuming applications to override the default implementation and allow it to be
		/// resolved from the container correctly for both the <see cref="IUmbrellaHostingEnvironment"/> and <see cref="IUmbrellaWebHostingEnvironment"/> interfaces.
		/// </typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="frontEndCompressionMiddlewareOptionsBuilder">The optional <see cref="FrontEndCompressionMiddlewareOptions"/> builder.</param>
		/// <param name="bundleUtilityOptionsBuilder">The optional <see cref="BundleUtilityOptions"/> builder.</param>
		/// <param name="webpackBundleUtilityOptionsBuilder">The optional <see cref="WebpackBundleUtilityOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaLegacyWebUtilities<TUmbrellaWebHostingEnvironment>(
			this IServiceCollection services,
			Action<IServiceProvider, FrontEndCompressionMiddlewareOptions> frontEndCompressionMiddlewareOptionsBuilder = null,
			Action<IServiceProvider, BundleUtilityOptions> bundleUtilityOptionsBuilder = null,
			Action<IServiceProvider, WebpackBundleUtilityOptions> webpackBundleUtilityOptionsBuilder = null)

			where TUmbrellaWebHostingEnvironment : class, IUmbrellaWebHostingEnvironment
		{
			Guard.ArgumentNotNull(services, nameof(services));

			// Add the hosting environment as a singleton and then ensure the same instance is bound to both interfaces
			services.AddSingleton<TUmbrellaWebHostingEnvironment>();
			services.AddSingleton<IUmbrellaHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());
			services.AddSingleton<IUmbrellaWebHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());

			services.AddSingleton<IBundleUtility, BundleUtility>();
			services.AddSingleton<IWebpackBundleUtility, WebpackBundleUtility>();

			services.AddSingleton<CleanupIDisposableMiddleware>();
			services.AddSingleton<DebugRequestMiddleware>();
			services.AddSingleton<FrontEndCompressionMiddleware>();

			// Options
			services.ConfigureUmbrellaOptions(frontEndCompressionMiddlewareOptionsBuilder);
			services.ConfigureUmbrellaOptions(bundleUtilityOptionsBuilder);
			services.ConfigureUmbrellaOptions(webpackBundleUtilityOptionsBuilder);

			return services;
		}
	}
}