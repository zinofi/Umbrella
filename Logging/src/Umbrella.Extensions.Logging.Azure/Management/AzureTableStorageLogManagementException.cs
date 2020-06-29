using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Extensions.Logging.Azure.Management
{
    public class AzureTableStorageLogManagementException : UmbrellaException
    {
        public AzureTableStorageLogManagementException(string message)
            : base(message)
        {
        }

        public AzureTableStorageLogManagementException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}