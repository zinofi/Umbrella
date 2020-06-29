using System;
using System.Runtime.CompilerServices;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Bundling;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling.Options;
using Umbrella.WebUtilities.Hosting.Options;
using Umbrella.WebUtilities.Http;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;
using Umbrella.WebUtilities.Security;

[assembly: InternalsVisibleTo("Umbrella.WebUtilities.Test")]

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.WebUtilities"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="bundleUtilityOptionsBuilder">The optional <see cref="BundleUtilityOptions"/> builder.</param>
		/// <param name="frontEndCompressionMiddlewareOptionsBuilder">The optional <see cref="FrontEndCompressionMiddlewareOptions"/> builder.</param>
		/// <param name="webpackBundleUtilityOptionsBuilder">The optional <see cref="WebpackBundleUtilityOptions"/> builder.</param>
		/// <param name="internetExplorerCacheHeaderMiddlewareOptionsBuilder">The optional <see cref="InternetExplorerCacheHeadersMiddlewareOptions"/> builder.</param>
		/// <param name="umbrellaWebHostingEnvironmentOptionsBuilder">The optional <see cref="UmbrellaWebHostingEnvironmentOptions"/> builder.</param>
		/// <param name="queryStringParameterToHttpHeaderMiddlewareOptionsBuilder">The optional <see cref="QueryStringParameterToHttpHeaderMiddlewareOptions"/> builder.</param>
		/// <param name="multiTenantSessionContextMiddlewareOptionsBuilder">The optional <see cref="MultiTenantSessionContextMiddlewareOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaWebUtilities(
			this IServiceCollection services,
			Action<IServiceProvider, BundleUtilityOptions> bundleUtilityOptionsBuilder = null,
			Action<IServiceProvider, FrontEndCompressionMiddlewareOptions> frontEndCompressionMiddlewareOptionsBuilder = null,
			Action<IServiceProvider, WebpackBundleUtilityOptions> webpackBundleUtilityOptionsBuilder = null,
			Action<IServiceProvider, InternetExplorerCacheHeadersMiddlewareOptions> internetExplorerCacheHeaderMiddlewareOptionsBuilder = null,
			Action<IServiceProvider, UmbrellaWebHostingEnvironmentOptions> umbrellaWebHostingEnvironmentOptionsBuilder = null,
			Action<IServiceProvider, QueryStringParameterToHttpHeaderMiddlewareOptions> queryStringParameterToHttpHeaderMiddlewareOptionsBuilder = null,
			Action<IServiceProvider, MultiTenantSessionContextMiddlewareOptions> multiTenantSessionContextMiddlewareOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IBundleUtility, BundleUtility>();
			services.AddSingleton<IHttpHeaderValueUtility, HttpHeaderValueUtility>();
			services.AddSingleton<IWebpackBundleUtility, WebpackBundleUtility>();

			services.AddScoped<NonceContext>();

			// Options
			services.ConfigureUmbrellaOptions(bundleUtilityOptionsBuilder);
			services.ConfigureUmbrellaOptions(frontEndCompressionMiddlewareOptionsBuilder);
			services.ConfigureUmbrellaOptions(webpackBundleUtilityOptionsBuilder);
			services.ConfigureUmbrellaOptions(internetExplorerCacheHeaderMiddlewareOptionsBuilder);
			services.ConfigureUmbrellaOptions(umbrellaWebHostingEnvironmentOptionsBuilder);
			services.ConfigureUmbrellaOptions(queryStringParameterToHttpHeaderMiddlewareOptionsBuilder);
			services.ConfigureUmbrellaOptions(multiTenantSessionContextMiddlewareOptionsBuilder);

			return services;
		}
	}
}