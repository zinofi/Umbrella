using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Utilities.Hosting;

//Set the namespace here to SimpleInjector so that the extension methods will appear automatically
namespace SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddUmbrellaLegacyWebUtilities(this Container container)
        {
            container.Register<IUmbrellaHostingEnvironment, UmbrellaHostingEnvironment>(Lifestyle.Singleton);
        }
    }
}