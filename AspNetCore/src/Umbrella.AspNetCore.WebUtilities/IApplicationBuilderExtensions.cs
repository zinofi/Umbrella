using System;
using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
	public static class IApplicationBuilderExtensions
	{
		// TODO: Create an Options class, DI it
		public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(
			this IApplicationBuilder builder,
			string queryStringParameterName,
			string headerName,
			Func<string, string> valueTransformer = null) => builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>(queryStringParameterName, headerName, valueTransformer);

		public static IApplicationBuilder InternetExplorerCacheHeaderMiddleware(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<InternetExplorerCacheHeaderMiddleware>();
		}
	}
}