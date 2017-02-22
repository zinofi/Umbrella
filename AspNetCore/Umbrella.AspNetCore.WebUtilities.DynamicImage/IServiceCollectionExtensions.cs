using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAspNetCoreWebUtilitiesDynamicImage(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();
            services.AddSingleton<IDynamicImageResizer, DynamicImageResizer>();
            services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();
            services.AddSingleton<IDynamicImageUrlGenerator, DynamicImageUrlGenerator>();

            return services;
        }
    }
}