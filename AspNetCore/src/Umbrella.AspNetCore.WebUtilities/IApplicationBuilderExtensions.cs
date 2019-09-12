using System;
using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.AspNetCore.WebUtilities.Middleware.Options;

namespace Microsoft.AspNetCore.Builder
{
	public static class IApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseUmbrellaCleanupIDisposable(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<CleanupIDisposableMiddleware>();
		}

		public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(
			this IApplicationBuilder builder,
			string queryStringParameterName,
			string headerName,
			Func<string, string> valueTransformer = null)
		{
			return builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>(queryStringParameterName, headerName, valueTransformer);
		}

		public static IApplicationBuilder UseUmbrellaInternetExplorerCacheHeaders(this IApplicationBuilder builder, Action<InternetExplorerCacheHeaderOptions> optionsBuilder)
		{
			return builder.UseMiddleware<InternetExplorerCacheHeaderMiddleware>(optionsBuilder);
		}
	}
}