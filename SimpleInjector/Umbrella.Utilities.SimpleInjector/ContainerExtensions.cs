using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;

namespace SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void UseUmbrellaUtilities(this Container container)
        {
            container.Register<IEmailBuilder, EmailBuilder>(Lifestyle.Singleton);
        }
    }
}
