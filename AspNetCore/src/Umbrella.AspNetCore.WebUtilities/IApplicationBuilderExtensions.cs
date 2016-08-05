using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Middleware;

namespace Umbrella.AspNetCore.WebUtilities
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCleanupIDisposable(this IApplicationBuilder builder) => builder.UseMiddleware<CleanupIDisposableMiddleware>();

        public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(this IApplicationBuilder builder,
            string queryStringParamaterName,
            string headerName,
            Func<string, string> valueTransformer = null)
        {
            return builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>(queryStringParamaterName, headerName, valueTransformer);
        }
    }
}