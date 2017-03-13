using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.DynamicImage;
using Umbrella.DynamicImage.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreDynamicImage(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicImageUrlGenerator, DynamicImageUrlGenerator>();

            return services;
        }
    }
}