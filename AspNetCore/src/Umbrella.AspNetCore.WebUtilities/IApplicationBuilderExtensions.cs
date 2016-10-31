using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.AspNetCore.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUmbrellaCleanupIDisposable(this IApplicationBuilder builder) => builder.UseMiddleware<CleanupIDisposableMiddleware>();

        public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(this IApplicationBuilder builder,
            string queryStringParamaterName,
            string headerName,
            Func<string, string> valueTransformer = null)
        {
            return builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>(queryStringParamaterName, headerName, valueTransformer);
        }

        public static IApplicationBuilder UseUmbrellaInternetExplorerCacheHeaders(this IApplicationBuilder builder, Action<InternetExplorerCacheHeaderOptions> config) => builder.UseMiddleware<InternetExplorerCacheHeaderMiddleware>(config);
    }
}