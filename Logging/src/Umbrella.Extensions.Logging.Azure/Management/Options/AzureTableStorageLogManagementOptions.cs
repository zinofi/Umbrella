using System.Collections.Generic;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Extensions.Logging.Azure.Management.Options
{
	public class AzureTableStorageLogManagementOptions : IValidatableUmbrellaOptions
	{
		public int CacheLifetimeMinutes { get; set; } = 30;
		public string AzureStorageConnectionString { get; set; }
		public List<AzureTableStorageLogDataSource> DataSources { get; set; } = new List<AzureTableStorageLogDataSource>();

		public void Validate()
		{
			Guard.ArgumentNotNullOrWhiteSpace(AzureStorageConnectionString, nameof(AzureStorageConnectionString));
			Guard.ArgumentInRange(CacheLifetimeMinutes, nameof(CacheLifetimeMinutes), 0);
		}
	}
}