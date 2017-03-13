using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.SoundInTheory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDynamicImageSoundInTheory(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicImageResizer, DynamicImageResizer>();

            return services;
        }
    }
}