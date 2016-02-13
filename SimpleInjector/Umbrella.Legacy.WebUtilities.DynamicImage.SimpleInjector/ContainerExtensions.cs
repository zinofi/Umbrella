using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using Umbrella.WebUtilities.DynamicImage.SimpleInjector;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void UseUmbrellaLegacyDynamicImage(this Container container)
        {
            container.UseUmbrellaDynamicImage();

            container.Register<IDynamicImageUrlGenerator, DynamicImageUrlGenerator>(Lifestyle.Singleton);
        }
    }
}