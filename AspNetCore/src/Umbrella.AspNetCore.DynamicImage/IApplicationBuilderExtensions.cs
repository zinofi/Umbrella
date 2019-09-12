using System;
using Umbrella.AspNetCore.DynamicImage.Middleware;
using Umbrella.AspNetCore.DynamicImage.Middleware.Options;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
	// TODO: Sort this	
	/// <summary>
	/// Extension methods used to register Middleware for the <see cref="Umbrella.AspNetCore.DynamicImage"/> package with a specified <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.DynamicImage"/> Middleware to the specified <see cref="IApplicationBuilder"/>.
		/// </summary>
		/// <param name="builder">The application builder to which the Middleware will be added.</param>
		/// <param name="optionsBuilder">The optional <see cref="DynamicImageMiddlewareOptions"/> builder.</param>
		/// <returns>The <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="builder"/> is null.</exception>
		public static IApplicationBuilder UseUmbrellaDynamicImage(this IApplicationBuilder builder, Action<DynamicImageMiddlewareOptions> optionsBuilder = null)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			// TODO: The options will never to be registered with DI and neither will the middleware. This is a behaviour difference from the legacy middleware and options which are
			// both register with DI as Singletons. Think about what impact this has, if any.

			// PROs
			// We can have multiple instances of the same middlware in the pipeline with different options.
			// This is actually much better than polluting the DI container with Middleware!

			// CONs
			// ???

			// Maybe the best solution here would to add a <remarks> doc above explaining this difference and that when using other DI container other than the default MS one,
			// e.g. AutoFac, don't expect that frameworks UseMiddlewareFromContainer<T> methods to work as it will be unaware they exist.

			return builder.UseMiddleware<DynamicImageMiddleware>(optionsBuilder);
		}
	}
}