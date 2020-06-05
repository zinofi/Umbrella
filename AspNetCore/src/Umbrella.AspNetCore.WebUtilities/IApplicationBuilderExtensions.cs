using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// Extentions methods for the <see cref="IApplicationBuilder" /> type.
	/// These methods will usually be called when configuring the middleware pipeline in Startup.cs.
	/// </summary>
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the <see cref="QueryStringParameterToHttpHeaderMiddleware" /> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>();
		}

		/// <summary>
		/// Adds the <see cref="InternetExplorerCacheHeadersMiddleware" /> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaInternetExplorerCacheHeaders(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<InternetExplorerCacheHeadersMiddleware>();
		}
	}
}