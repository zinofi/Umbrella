using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.Azure.Configuration
{
    public class AzureTableStorageLogAppenderOptions
    {
        public string Name { get; set; }
        public string TablePrefix { get; set; }
        public AzureTableStorageLogAppenderType AppenderType { get; set; }
    }
}
