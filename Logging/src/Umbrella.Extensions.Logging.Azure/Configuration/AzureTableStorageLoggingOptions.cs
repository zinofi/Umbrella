using System.Collections.Generic;

namespace Umbrella.Extensions.Logging.Azure.Configuration
{
	/// <summary>
	/// Specifies the options for use with the Azure Table logging infrastructure.
	/// </summary>
	public class AzureTableStorageLoggingOptions
    {
		/// <summary>
		/// The table name separator
		/// </summary>
		public const string TableNameSeparator = "xxxxxx";

		/// <summary>
		/// Gets or sets a value indicating whether to log errors to the console for diagnostic purposes. Defaults to <see langword="false"/>.
		/// </summary>
		public bool LogErrorsToConsole { get; set; }

		/// <summary>
		/// Gets or sets the appenders.
		/// </summary>
		public List<AzureTableStorageLogAppenderOptions> Appenders { get; set; } = new List<AzureTableStorageLogAppenderOptions>();
    }
}