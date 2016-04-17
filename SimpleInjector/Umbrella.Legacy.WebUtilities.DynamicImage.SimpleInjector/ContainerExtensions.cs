using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

//Set the namespace here to SimpleInjector so that the extension methods will appear automatically
namespace SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddUmbrellaLegacyDynamicImage(this Container container)
        {
            container.AddUmbrellaDynamicImage();

            container.Register<IDynamicImageUrlGenerator, DynamicImageUrlGenerator>(Lifestyle.Singleton);
        }
    }
}