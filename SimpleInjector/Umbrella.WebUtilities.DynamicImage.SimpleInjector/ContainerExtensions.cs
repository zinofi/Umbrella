using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.WebUtilities.DynamicImage.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void UseUmbrellaDynamicImage(this Container container)
        {
            container.Register<IDynamicImageCache, DynamicImageDiskCache>(Lifestyle.Singleton);
            container.Register<IDynamicImageResizer, DynamicImageResizer>(Lifestyle.Singleton);
            container.Register<IDynamicImageUtility, DynamicImageUtility>(Lifestyle.Singleton);
        }
    }
}