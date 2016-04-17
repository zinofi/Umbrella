using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

//Set the namespace here to SimpleInjector so that the extension methods will appear automatically
namespace SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddUmbrellaDynamicImage(this Container container)
        {
            container.Register<IDynamicImageCache, DynamicImageDiskCache>(Lifestyle.Singleton);
            container.Register<IDynamicImageResizer, DynamicImageResizer>(Lifestyle.Singleton);
            container.Register<IDynamicImageUtility, DynamicImageUtility>(Lifestyle.Singleton);
        }
    }
}