using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.WebUtilities.Http;

namespace Umbrella.WebUtilities
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