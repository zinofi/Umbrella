namespace Umbrella.Extensions.Logging.Azure.Configuration
{
	/// <summary>
	/// Options for use with logging appenders that write to Azure Table Storage.
	/// </summary>
	public class AzureTableStorageLogAppenderOptions
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AzureTableStorageLogAppenderOptions"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="tablePrefix">The table prefix.</param>
		/// <param name="appenderType">Type of the appender.</param>
		public AzureTableStorageLogAppenderOptions(string name, string tablePrefix, AzureTableStorageLogAppenderType appenderType)
		{
			Name = name;
			TablePrefix = tablePrefix;
			AppenderType = appenderType;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the table prefix.
		/// </summary>
		public string TablePrefix { get; } = null!;

		/// <summary>
		/// Gets the type of the appender.
		/// </summary>
		public AzureTableStorageLogAppenderType AppenderType { get; }
    }
}
