using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void UseUmbrellaLegacyWebUtilities(this Container container)
        {
            container.Register<IUmbrellaHostingEnvironment, UmbrellaHostingEnvironment>(Lifestyle.Singleton);
        }
    }
}