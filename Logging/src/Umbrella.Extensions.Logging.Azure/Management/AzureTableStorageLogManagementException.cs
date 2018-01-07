using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Extensions.Logging.Azure.Management
{
    public class AzureTableStorageLogManagementException : Exception
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