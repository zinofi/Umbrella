using System.Collections.Generic;

namespace Umbrella.Extensions.Logging.Azure.Configuration
{
	public class AzureTableStorageLoggingOptions
    {
        public const string TableNameSeparator = "xxxxxx";
        public bool LogErrorsToConsole { get; set; }
        public List<AzureTableStorageLogAppenderOptions> Appenders { get; set; } = new List<AzureTableStorageLogAppenderOptions>();
    }
}