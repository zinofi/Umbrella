using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Extensions.Logging.Log4Net.Azure.Configuration;

namespace Umbrella.Extensions.Logging.Log4Net.Azure.Management.Configuration
{
    public class AzureTableStorageLogDataSource
    {
        public string TablePrefix { get; set; }
        public AzureTableStorageLogAppenderType AppenderType { get; set; }
        public string CategoryName { get; set; }
    }
}