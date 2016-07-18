using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.Log4Net.Azure.Configuration
{
    public class AzureTableStorageLoggingOptions
    {
        public bool LogErrorsToConsole { get; set; }
        public List<AzureTableStorageLogAppenderOptions> Appenders { get; set; } = new List<AzureTableStorageLogAppenderOptions>();
    }
}
