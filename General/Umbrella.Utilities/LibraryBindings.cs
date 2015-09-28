using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Utilities
{
    public static class LibraryBindings
    {
        public static StandardKernel DependencyResolver { get; set; }

        static LibraryBindings()
        {
            DependencyResolver = new StandardKernel();

            //Get all other library assemblies and find the module types that are not generic
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("Umbrella.")).SelectMany(x => x.ExportedTypes).Where(x => !x.IsGenericType && typeof(INinjectModule).IsAssignableFrom(x));

            var instances = types.Select(x => Activator.CreateInstance(x)).OfType<INinjectModule>();

            DependencyResolver.Load(instances);
        }
    }
}