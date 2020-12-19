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
		/// Initializes a new instance of the <see cref="AzureTableStorageLogDataSource"/> class.
		/// </summary>
		/// <param name="tablePrefix">The table prefix.</param>
		/// <param name="appenderType">Type of the appender.</param>
		/// <param name="categoryName">Name of the category.</param>
		public AzureTableStorageLogDataSource(string tablePrefix, AzureTableStorageLogAppenderType appenderType, string categoryName)
		{
			TablePrefix = tablePrefix;
			AppenderType = appenderType;
			CategoryName = categoryName;
		}

		/// <summary>
		/// Gets the table prefix.
		/// </summary>
		public string TablePrefix { get; }

		/// <summary>
		/// Gets the type of the appender.
		/// </summary>
		public AzureTableStorageLogAppenderType AppenderType { get; }

		/// <summary>
		/// Gets the name of the category.
		/// </summary>
		public string CategoryName { get; }
	}
}