using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Extensions.Logging.Azure.Configuration;

namespace Umbrella.Extensions.Logging.Azure.Management.Configuration
{
    public class AzureTableStorageLogDataSource
    {
        public string TablePrefix { get; set; }
        public AzureTableStorageLogAppenderType AppenderType { get; set; }
        public string CategoryName { get; set; }
    }
}