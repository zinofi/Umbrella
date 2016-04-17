using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;

//Set the namespace here to SimpleInjector so that the extension methods will appear automatically
namespace SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddUmbrellaUtilities(this Container container)
        {
            container.Register<IEmailBuilder, EmailBuilder>();
        }
    }
}
