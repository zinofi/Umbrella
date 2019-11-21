using Umbrella.Extensions.Logging.Azure.Configuration;

namespace Umbrella.Extensions.Logging.Azure.Management.Configuration
{
	/// <summary>
	/// A logging data source. This encapsulates the type of the logs being stored, i.e. client or server,
	/// the table prefix used when crearing tables in Azure Storage, and the friendly category name used for display purposes.
	/// </summary>
	public class AzureTableStorageLogDataSource
	{
		/// <summary>
		/// Gets or sets the table prefix.
		/// </summary>
		public string TablePrefix { get; set; }

		/// <summary>
		/// Gets or sets the type of the appender.
		/// </summary>
		public AzureTableStorageLogAppenderType AppenderType { get; set; }

		/// <summary>
		/// Gets or sets the name of the category.
		/// </summary>
		public string CategoryName { get; set; }
	}
}