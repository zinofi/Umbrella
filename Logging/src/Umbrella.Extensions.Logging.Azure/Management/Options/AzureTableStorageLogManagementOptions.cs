using System.Collections.Generic;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Extensions.Logging.Azure.Management.Options
{
	/// <summary>
	/// The options for the <see cref="AzureTableStorageLogManager"/>.
	/// </summary>
	/// <seealso cref="IValidatableUmbrellaOptions" />
	public class AzureTableStorageLogManagementOptions : IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the cache lifetime minutes.
		/// </summary>
		public int CacheLifetimeMinutes { get; set; } = 30;

		/// <summary>
		/// Gets or sets the azure storage connection string.
		/// </summary>
		public string AzureStorageConnectionString { get; set; } = null!;

		/// <summary>
		/// Gets the data sources.
		/// </summary>
		public List<AzureTableStorageLogDataSource> DataSources { get; } = new List<AzureTableStorageLogDataSource>();

		/// <inheritdoc />
		public void Validate()
		{
			Guard.ArgumentNotNullOrWhiteSpace(AzureStorageConnectionString, nameof(AzureStorageConnectionString));
			Guard.ArgumentInRange(CacheLifetimeMinutes, nameof(CacheLifetimeMinutes), 0);
		}
	}
}