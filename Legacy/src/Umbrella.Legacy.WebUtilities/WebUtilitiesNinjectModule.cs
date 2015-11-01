using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Legacy.WebUtilities
{
    public class WebUtilitiesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IHostingEnvironment>().To<HostingEnvironment>();
        }
    }
}
