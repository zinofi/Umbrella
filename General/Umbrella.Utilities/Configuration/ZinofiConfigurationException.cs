using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbrella.Utilities.Configuration.Exceptions
{
    public class UmbrellaConfigurationException : ApplicationException
    {
        public UmbrellaConfigurationException()
            : base("A generic Umbrella configuration section has occurred.")
        {
        }

        public UmbrellaConfigurationException(string message)
            : base(message)
        {
        }
    }
}