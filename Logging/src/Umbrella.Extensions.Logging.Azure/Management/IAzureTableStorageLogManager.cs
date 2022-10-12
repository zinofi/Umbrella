// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Azure.Data.Tables;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;

namespace Umbrella.Extensions.Logging.Azure.Management;

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
	Task<(IReadOnlyCollection<AzureTableStorageLogDataSource> items, int totalCount)> FindAllDataSourceByOptionsAsync(AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds all log tables by table prefix using the specified options.
	/// </summary>
	/// <param name="tablePrefix">The table prefix.</param>
	/// <param name="options">The options.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple containing a list of tables together with the total count of all tables with the same <paramref name="tablePrefix"/>.</returns>
	Task<(IReadOnlyCollection<AzureTableStorageLogTable> items, int totalCount)> FindAllLogTableByTablePrefixAndOptionsAsync(string tablePrefix, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds all table entities using the specified options.
	/// </summary>
	/// <param name="tablePrefix">The table prefix.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="options">The options.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple containing the appender type, a list of tables together and the total count of all tables with the same <paramref name="tablePrefix"/> and <paramref name="tableName"/>.</returns>
	Task<(AzureTableStorageLogAppenderType? appenderType, IReadOnlyCollection<TEntity> items, int totalCount)> FindAllTableEntityByOptionsAsync<TEntity>(string tablePrefix, string tableName, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
		where TEntity : class, ITableEntity, new();

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