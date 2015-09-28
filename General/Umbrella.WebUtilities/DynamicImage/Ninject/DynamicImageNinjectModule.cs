using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.WebUtilities.DynamicImage.Ninject
{
    public class DynamicImageNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDynamicImageCache>().To<DynamicImageNullCache>();
            Bind<IDynamicImageResizer>().To<DynamicImageResizer>();
        }
    }
}
