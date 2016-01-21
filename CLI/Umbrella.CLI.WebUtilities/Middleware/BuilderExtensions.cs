using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.CLI.WebUtilities.Middleware
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseCleanupIDisposable(this IApplicationBuilder app) => app.UseMiddleware<CleanupIDisposableMiddleware>();
    }
}
