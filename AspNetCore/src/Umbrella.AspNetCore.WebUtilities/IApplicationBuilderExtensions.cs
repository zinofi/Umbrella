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
        public static IApplicationBuilder UseCleanupIDisposable(this IApplicationBuilder app) => app.UseMiddleware<CleanupIDisposableMiddleware>();
    }
}
