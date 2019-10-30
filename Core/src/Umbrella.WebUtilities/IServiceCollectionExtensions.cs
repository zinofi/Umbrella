using System;
using System.Runtime.CompilerServices;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Http;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

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
		/// <param name="frontEndCompressionMiddlewareOptionsBuilder">The optional <see cref="FrontEndCompressionMiddlewareOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaWebUtilities(
			this IServiceCollection services,
			Action<IServiceProvider, FrontEndCompressionMiddlewareOptions> frontEndCompressionMiddlewareOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IHttpHeaderValueUtility, HttpHeaderValueUtility>();

			// Options
			services.ConfigureUmbrellaOptions(frontEndCompressionMiddlewareOptionsBuilder);

			return services;
		}
	}
}