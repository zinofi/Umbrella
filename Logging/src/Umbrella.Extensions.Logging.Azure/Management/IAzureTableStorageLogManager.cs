using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;

namespace Umbrella.Extensions.Logging.Azure.Management
{
	/// <summary>
	/// A utility for managing logs stored using Azure Table Storage.
	/// </summary>
	public interface IAzureTableStorageLogManager
	{
		/// <summary>
		/// Finds all data sources using the specified options.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the data sources results and the total count of all data sources.</returns>
		Task<(List<AzureTableStorageLogDataSource> Items, int TotalCount)> FindAllDataSourceByOptionsAsync(AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default);


		Task<(List<AzureTableStorageLogTable> Items, int TotalCount)> FindAllLogTableByTablePrefixAndOptionsAsync(string tablePrefix, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default);


		Task<(AzureTableStorageLogAppenderType? AppenderType, List<TableEntity> Items, int TotalCount)> FindAllTableEntityByOptionsAsync(string tablePrefix, string tableName, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a table using its name.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="AzureTableStorageLogDeleteOperationResult"/> of the operation.</returns>
		Task<AzureTableStorageLogDeleteOperationResult> DeleteTableByNameAsync(string tableName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Clears the table name cache. This is needed when new tables may have been created in table storage which do not exist in the cache.
		/// </summary>
		/// <param name="tablePrefix">The table prefix.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> which completes when the operation has completed.</returns>
		Task ClearTableNameCacheAsync(string tablePrefix, CancellationToken cancellationToken = default);
	}
}