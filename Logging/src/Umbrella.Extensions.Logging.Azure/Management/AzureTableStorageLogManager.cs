// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;
using Umbrella.Extensions.Logging.Azure.Management.Options;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Comparers;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Extensions.Logging.Azure.Management;

/// <summary>
/// A utility for managing logs stored using Azure Table Storage.
/// </summary>
/// <seealso cref="IAzureTableStorageLogManager" />
public class AzureTableStorageLogManager : IAzureTableStorageLogManager
{
	#region Private Inner Classes
	private class LogEntryMetaData
	{
		public LogEntryMetaData(string partitionKey, string rowKey, DateTime? eventTimeStamp, string level)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;
			EventTimeStamp = eventTimeStamp;
			Level = level;
		}

		public string PartitionKey { get; }
		public string RowKey { get; }
		public DateTime? EventTimeStamp { get; }
		public string Level { get; }
	}
	#endregion

	#region Private Members
	private static readonly Dictionary<string, string> _normalizedDataSourceKeyDictionary = new()
	{
		[nameof(AzureTableStorageLogDataSource.AppenderType)] = Normalize(nameof(AzureTableStorageLogDataSource.AppenderType)),
		[nameof(AzureTableStorageLogDataSource.CategoryName)] = Normalize(nameof(AzureTableStorageLogDataSource.CategoryName)),
		[nameof(AzureTableStorageLogDataSource.TablePrefix)] = Normalize(nameof(AzureTableStorageLogDataSource.TablePrefix))
	};
	private static readonly Dictionary<string, string> _normalizedTableModelKeyDictionary = new()
	{
		[nameof(AzureTableStorageLogTable.Date)] = Normalize(nameof(AzureTableStorageLogTable.Date)),
		[nameof(AzureTableStorageLogTable.Name)] = Normalize(nameof(AzureTableStorageLogTable.Name))
	};
	private static readonly Dictionary<string, string> _normalizedLogEntryMetaDataKeyDictionary = new()
	{
		[nameof(LogEntryMetaData.EventTimeStamp)] = Normalize(nameof(LogEntryMetaData.EventTimeStamp)),
		[nameof(LogEntryMetaData.Level)] = Normalize(nameof(LogEntryMetaData.Level)),
	};
	private static readonly GenericEqualityComparer<CloudTable, string> _cloudTableEqualityComparer = new(x => x.Name);
	private static readonly string[] _dateSeparatorArray = new[] { AzureTableStorageLoggingOptions.TableNameSeparator };
	private static readonly string _cacheKeyPrefix = typeof(AzureTableStorageLogManager).FullName;
	#endregion

	#region Protected Properties		
	/// <summary>
	/// Gets the log.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the log management options.
	/// </summary>
	protected AzureTableStorageLogManagementOptions LogManagementOptions { get; }

	/// <summary>
	/// Gets the storage account.
	/// </summary>
	protected CloudStorageAccount StorageAccount { get; }

	/// <summary>
	/// Gets the memory cache.
	/// </summary>
	protected IMemoryCache MemoryCache { get; }

	/// <summary>
	/// Gets the distributed cache.
	/// </summary>
	protected IDistributedCache DistributedCache { get; }

	/// <summary>
	/// Gets the table list cache entry options.
	/// </summary>
	protected DistributedCacheEntryOptions TableListCacheEntryOptions { get; }

	/// <summary>
	/// Gets the log entry meta data cache entry options.
	/// </summary>
	protected MemoryCacheEntryOptions LogEntryMetaDataCacheEntryOptions { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="AzureTableStorageLogManager"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The options.</param>
	/// <param name="memoryCache">The memory cache.</param>
	/// <param name="distributedCache">The distributed cache.</param>
	public AzureTableStorageLogManager(ILogger<AzureTableStorageLogManager> logger,
		AzureTableStorageLogManagementOptions options,
		IMemoryCache memoryCache,
		IDistributedCache distributedCache)
	{
		Logger = logger;
		LogManagementOptions = options;
		StorageAccount = CloudStorageAccount.Parse(options.AzureStorageConnectionString);
		MemoryCache = memoryCache;
		DistributedCache = distributedCache;

		TableListCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes) };
		LogEntryMetaDataCacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes), Priority = CacheItemPriority.Low };
	}
	#endregion

	#region IAzureTableStorageLogManager Members
	/// <inheritdoc />
	public Task<(List<AzureTableStorageLogDataSource> Items, int TotalCount)> FindAllDataSourceByOptionsAsync(AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(options, nameof(options));

		try
		{
			IEnumerable<AzureTableStorageLogDataSource> lstDataSource = LogManagementOptions.DataSources;

			//Apply filters first
			if (options.Filters?.Count > 0)
			{
				foreach (var kvp in options.Filters)
				{
					string key = Normalize(kvp.Key);
					string value = Normalize(kvp.Value);

					if (!string.IsNullOrWhiteSpace(value))
					{
						if (key == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.AppenderType)])
							lstDataSource = lstDataSource.Where(x => x.AppenderType == value.ToEnum<AzureTableStorageLogAppenderType>());
						else if (key == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.CategoryName)])
							lstDataSource = lstDataSource.Where(x => Normalize(x.CategoryName).Contains(value));
						else if (key == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.TablePrefix)])
							lstDataSource = lstDataSource.Where(x => Normalize(x.TablePrefix).Contains(value));
					}
				}
			}

			int totalCount = lstDataSource.Count();

			//Apply sorting - always default
			string? sortBy = options.SortProperty;

			if (string.IsNullOrWhiteSpace(sortBy))
				sortBy = nameof(AzureTableStorageLogDataSource.CategoryName);

			sortBy = Normalize(sortBy!);

			if (sortBy == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.AppenderType)])
			{
				lstDataSource = lstDataSource.OrderBySortDirection(x => x.AppenderType, options.SortDirection);
			}
			else
			{
				lstDataSource = sortBy == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.CategoryName)]
				? lstDataSource.OrderBySortDirection(x => x.CategoryName, options.SortDirection)
				: sortBy == _normalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.TablePrefix)]
				? lstDataSource.OrderBySortDirection(x => x.TablePrefix, options.SortDirection)
				: (IEnumerable<AzureTableStorageLogDataSource>)lstDataSource.OrderByDescending(x => x.CategoryName);
			}

			//Apply pagination
			int itemsToSkip = (options.PageNumber - 1) * options.PageSize;

			List<AzureTableStorageLogDataSource> lstSortedFilteredItem = options.PageSize > 0
				? lstDataSource.Skip(itemsToSkip).Take(options.PageSize).ToList()
				: lstDataSource.ToList();

			return Task.FromResult((lstSortedFilteredItem, totalCount));
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { options }))
		{
			throw new AzureTableStorageLogManagementException("There was a problem accessing the log data sources.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<(List<AzureTableStorageLogTable> Items, int TotalCount)> FindAllLogTableByTablePrefixAndOptionsAsync(string tablePrefix, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));
		Guard.IsNotNull(options, nameof(options));

		try
		{
			//Using the DistributedCache to store the table models so that changes are reflected globally
			(IEnumerable<AzureTableStorageLogTable>? cacheItem, UmbrellaDistributedCacheException? exception) = await DistributedCache.GetOrCreateAsync(GenerateCacheKey(tablePrefix), async () =>
			{
				cancellationToken.ThrowIfCancellationRequested();

				CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

				TableContinuationToken? continuationToken = null;
				TableResultSegment? resultSegment = null;

				var hsResult = new HashSet<CloudTable>(_cloudTableEqualityComparer);

				do
				{
					cancellationToken.ThrowIfCancellationRequested();

					resultSegment = await tableClient.ListTablesSegmentedAsync(tablePrefix, continuationToken, cancellationToken).ConfigureAwait(false);

					foreach (CloudTable table in resultSegment.Results)
					{
						if (table is not null)
							_ = hsResult.Add(table);
					}

					continuationToken = resultSegment.ContinuationToken;
				}
				while (continuationToken is not null);

				if (hsResult.Count == 0)
					return null;

				var cbTableModel = new ConcurrentBag<AzureTableStorageLogTable>();

				hsResult.AsParallel().ForAll(x =>
				{
					//The date is stored as part of the table name in the format {tablePrefix}xxxxxx{yyyy}x{mm}x{dd}, e.g. CostsBudgITPortalServerxxxxxx2016-09-27
					string[]? strDateParts = x.Name.Split(_dateSeparatorArray, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1)?.Split('x');

					if (strDateParts?.Length == 3)
					{
						int year = int.Parse(strDateParts[0]);
						int month = int.Parse(strDateParts[1]);
						int day = int.Parse(strDateParts[2]);

						var dtDate = new DateTime(year, month, day);
						var tableModel = new AzureTableStorageLogTable(dtDate, x.Name);

						cbTableModel.Add(tableModel);
					}
				});

				return cbTableModel.ToList();
			}, () => TableListCacheEntryOptions).ConfigureAwait(false);

			int totalCount = cacheItem?.Count() ?? 0;

			if (cacheItem is null || totalCount is 0)
				return (new List<AzureTableStorageLogTable>(), 0);

			//Apply sorting - always default
			string? sortBy = options.SortProperty;

			if (string.IsNullOrWhiteSpace(sortBy))
				sortBy = nameof(AzureTableStorageLogTable.Date);

			sortBy = Normalize(sortBy!);

			cacheItem = sortBy == _normalizedTableModelKeyDictionary[nameof(AzureTableStorageLogTable.Date)]
				? cacheItem.OrderBySortDirection(x => x.Date, options.SortDirection)
				: sortBy == _normalizedTableModelKeyDictionary[nameof(AzureTableStorageLogTable.Name)]
				? cacheItem.OrderBySortDirection(x => x.Name, options.SortDirection)
				: (IEnumerable<AzureTableStorageLogTable>)cacheItem.OrderByDescending(x => x.Date);

			//Apply pagination
			int itemsToSkip = (options.PageNumber - 1) * options.PageSize;

			List<AzureTableStorageLogTable> lstSortedFilteredItem = options.PageSize > 0
				? cacheItem.Skip(itemsToSkip).Take(options.PageSize).ToList()
				: cacheItem.ToList();

			return (lstSortedFilteredItem, totalCount);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { tablePrefix, options }))
		{
			throw new AzureTableStorageLogManagementException($"There was a problem accessing the log table table data for table prefix {tablePrefix}", exc);
		}
	}

	/// <inheritdoc />
	public async Task<(AzureTableStorageLogAppenderType? AppenderType, List<TableEntity> Items, int TotalCount)> FindAllTableEntityByOptionsAsync(string tablePrefix, string tableName, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));
		Guard.IsNotNullOrWhiteSpace(tableName, nameof(tableName));
		Guard.IsNotNull(options, nameof(options));

		try
		{
			//Validate the tablePrefix
			var dataSource = LogManagementOptions.DataSources.Find(x => Normalize(x.TablePrefix) == Normalize(tablePrefix));

			if (dataSource is null)
				return (null, new List<TableEntity>(), 0);

			CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

			CloudTable table = tableClient.GetTableReference(tableName);

			//Build the query we will use to populate the metadata cache
			TableQuery<DynamicTableEntity> cacheQuery = new TableQuery<DynamicTableEntity>().Select(new[] { nameof(LogEntryMetaData.EventTimeStamp), nameof(LogEntryMetaData.Level) });

			bool cacheFirstBuild = false;
			string cacheKey = GenerateCacheKey(tableName);
			TableContinuationToken? continuationToken = null;
			TableQuerySegment<DynamicTableEntity>? querySegment = null;

			//Try and get existing cache items - if there are none build the cache
			List<LogEntryMetaData> lstMetaData = await MemoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
			{
				cacheFirstBuild = true;

				_ = cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

				var lstResult = new List<LogEntryMetaData>();

				do
				{
					querySegment = await table.ExecuteQuerySegmentedAsync(cacheQuery, continuationToken, cancellationToken).ConfigureAwait(false);

					foreach (DynamicTableEntity entity in querySegment.Results)
					{
						var result = new LogEntryMetaData(entity.PartitionKey, entity.RowKey, entity[nameof(LogEntryMetaData.EventTimeStamp)].DateTime, entity[nameof(LogEntryMetaData.Level)].StringValue);

						lstResult.Add(result);
					}

					continuationToken = querySegment.ContinuationToken;
				}
				while (continuationToken is not null);

				return lstResult;
			}).ConfigureAwait(false);

			if (!cacheFirstBuild)
			{
				//Check ATS for any new rows added since the cache was populated but only if
				//we haven't just populated the cache in this request.
				//Get the last item in the cache
				LogEntryMetaData? entry = lstMetaData.LastOrDefault();

				if (entry is not null)
				{
					var lstNewEntries = new List<LogEntryMetaData>();

					cacheQuery = cacheQuery.Where(
						TableQuery.CombineFilters(
							TableQuery.GenerateFilterCondition(nameof(entry.PartitionKey), QueryComparisons.GreaterThanOrEqual, entry.PartitionKey),
							TableOperators.And,
							TableQuery.GenerateFilterCondition(nameof(entry.RowKey), QueryComparisons.GreaterThan, entry.RowKey)));

					do
					{
						querySegment = await table.ExecuteQuerySegmentedAsync(cacheQuery, continuationToken, cancellationToken).ConfigureAwait(false);

						foreach (var entity in querySegment.Results)
						{
							var result = new LogEntryMetaData(entity.PartitionKey, entity.RowKey, entity[nameof(LogEntryMetaData.EventTimeStamp)].DateTime, entity[nameof(LogEntryMetaData.Level)].StringValue);

							lstNewEntries.Add(result);
						}

						continuationToken = querySegment.ContinuationToken;
					}
					while (continuationToken is not null);

					//Append the new entries to the current list - these will automatically end up in the cache.
					lstMetaData.AddRange(lstNewEntries);
				}
			}

			int totalCount = lstMetaData.Count();

			//Apply sorting - always default
			string? sortBy = options.SortProperty;

			if (string.IsNullOrWhiteSpace(sortBy))
				sortBy = nameof(LogEntryMetaData.EventTimeStamp);

			sortBy = Normalize(sortBy!);

			IEnumerable<LogEntryMetaData> results = lstMetaData.ToList();

			results = sortBy == _normalizedLogEntryMetaDataKeyDictionary[nameof(LogEntryMetaData.EventTimeStamp)]
				? results.OrderBySortDirection(x => x.EventTimeStamp, options.SortDirection)
				: sortBy == _normalizedLogEntryMetaDataKeyDictionary[nameof(LogEntryMetaData.Level)]
				? results.OrderBySortDirection(x => x.Level, options.SortDirection)
				: (IEnumerable<LogEntryMetaData>)results.OrderByDescending(x => x.EventTimeStamp);

			//Apply pagination
			if (options.PageSize > 0)
			{
				int itemsToSkip = (options.PageNumber - 1) * options.PageSize;
				results = results.Skip(itemsToSkip).Take(options.PageSize);
			}

			//Now we have the paginated / sorted / filtered metadata from the cache
			//we need to retrieve the full items from ATS. Seemingly you can't do multiple
			//point lookups in a batch so each item will have to be retrieved manually.
			var tableOperation = dataSource.AppenderType == AzureTableStorageLogAppenderType.Client
				? TableOperation.Retrieve<AzureLoggingClientEventEntity>
				: (Func<string, string, List<string>?, TableOperation>)TableOperation.Retrieve<AzureLoggingServerEventEntity>;

			var opResults = await Task.WhenAll(results.Select(x => table.ExecuteAsync(tableOperation(x.PartitionKey, x.RowKey, null)))).ConfigureAwait(false);

			var items = opResults.Select(x => x.Result).Cast<TableEntity>().ToList();

			return (dataSource.AppenderType, items, totalCount);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { tablePrefix, tableName, options }))
		{
			throw new AzureTableStorageLogManagementException($"There was a problem accessing the log table table data for table prefix {tablePrefix} and table name {tableName}", exc);
		}
	}

	/// <inheritdoc />
	public async Task<AzureTableStorageLogDeleteOperationResult> DeleteTableByNameAsync(string tableName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tableName, nameof(tableName));

		try
		{
			CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

			CloudTable table = tableClient.GetTableReference(tableName);

			bool exists = await table.ExistsAsync(cancellationToken).ConfigureAwait(false);

			if (!exists)
				return AzureTableStorageLogDeleteOperationResult.NotFound;

			//Using the IfExists variant here in case a race condition has resulted in the table already having been deleted by another user.
			_ = await table.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);

			//Remove the table from the cached list - potential for a race condition to mess with this but should be low probability.
			string tablePrefix = tableName.Split(_dateSeparatorArray, StringSplitOptions.RemoveEmptyEntries)[0];
			string listCacheKey = GenerateCacheKey(tablePrefix);

			var lstTableModel = await DistributedCache.GetAsync<List<AzureTableStorageLogTable>>(listCacheKey, cancellationToken).ConfigureAwait(false);

			if (lstTableModel is not null)
			{
				var itemToRemove = lstTableModel.Find(x => x.Name == tableName);

				if (itemToRemove is not null)
				{
					_ = lstTableModel.Remove(itemToRemove);
					await DistributedCache.SetAsync(listCacheKey, lstTableModel, TableListCacheEntryOptions, cancellationToken).ConfigureAwait(false);
				}
			}

			//Also need to remove the log entries from the Memory Cache
			MemoryCache.Remove(GenerateCacheKey(tableName));

			return AzureTableStorageLogDeleteOperationResult.Success;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { tableName }))
		{
			throw new AzureTableStorageLogManagementException($"There has been a problem deleting the table with name {tableName}", exc);
		}
	}

	/// <inheritdoc />
	public async Task ClearTableNameCacheAsync(string tablePrefix, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));

		try
		{
			string cacheKey = GenerateCacheKey(tablePrefix);

			await DistributedCache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { tablePrefix }))
		{
			throw new AzureTableStorageLogManagementException($"There has been a problem clearing the cache for the table prefix {tablePrefix}", exc);
		}
	}
	#endregion

	#region Private Static Methods
	private static string Normalize(string value) => value.Trim().ToUpperInvariant().Normalize();
	#endregion

	#region Private Methods
	private string GenerateCacheKey(string key) => Normalize($"{_cacheKeyPrefix}:{key}");
	#endregion
}