using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Legacy.Utilities.Configuration.Exceptions
{
    public class UmbrellaConfigurationException : UmbrellaException
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