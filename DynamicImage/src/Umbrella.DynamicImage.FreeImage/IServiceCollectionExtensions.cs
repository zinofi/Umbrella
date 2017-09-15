using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.FreeImage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageFreeImage(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicImageResizer, DynamicImageResizer>();

            return services;
        }
    }
}