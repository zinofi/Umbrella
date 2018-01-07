using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Extensions.Logging.Azure.Management.Configuration
{
    public class AzureTableStorageLogManagementOptions
    {
        public int CacheLifetimeMinutes { get; set; } = 30;
        public string AzureStorageConnectionString { get; set; }
        public List<AzureTableStorageLogDataSource> DataSources { get; set; } = new List<AzureTableStorageLogDataSource>();
    }
}