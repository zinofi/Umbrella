using System;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// Extension methods used to register Middleware for the <see cref="Umbrella.AspNetCore.WebUtilities.DynamicImage"/> package with a specified <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.WebUtilities.DynamicImage"/> Middleware to the specified <see cref="IApplicationBuilder"/>.
		/// </summary>
		/// <param name="builder">The application builder to which the Middleware will be added.</param>
		/// <returns>The <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="builder"/> is null.</exception>
		public static IApplicationBuilder UseUmbrellaDynamicImage(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder;
		}
	}
}