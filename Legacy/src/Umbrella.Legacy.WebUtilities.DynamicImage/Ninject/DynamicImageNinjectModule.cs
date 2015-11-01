using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Ninject
{
    public class DynamicImageNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDynamicImageUrlGenerator>().To<DynamicImageUrlGenerator>();
        }
    }
}
