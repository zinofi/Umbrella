using System;
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
		// TODO: Create an Options class, DI it		
		/// <summary>
		/// Adds the <see cref="QueryStringParameterToHttpHeaderMiddleware" /> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="queryStringParameterName">Name of the query string parameter.</param>
		/// <param name="headerName">Name of the header.</param>
		/// <param name="valueTransformer">The value transformer.</param>
		/// <returns></returns>
		public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(
			this IApplicationBuilder builder,
			string queryStringParameterName,
			string headerName,
			Func<string, string>? valueTransformer = null) => builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>(queryStringParameterName, headerName, valueTransformer);

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