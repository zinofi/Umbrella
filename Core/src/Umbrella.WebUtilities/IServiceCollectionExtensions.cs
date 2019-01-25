using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Umbrella.WebUtilities.Http;

[assembly: InternalsVisibleTo("Umbrella.WebUtilities.Test")]

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaWebUtilities(this IServiceCollection services)
        {
            services.AddSingleton<IHttpHeaderValueUtility, HttpHeaderValueUtility>();

            return services;
        }
    }
}