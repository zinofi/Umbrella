// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
	private class LogEntryMetaData : ITableEntity
	{
		public string PartitionKey { get; set; } = null!;
		public string RowKey { get; set; } = null!;
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public DateTime? EventTimeStamp { get; set; }
		public string Level { get; set; } = null!;
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
	private static readonly GenericEqualityComparer<TableItem, string> _cloudTableEqualityComparer = new(x => x.Name);
	private static readonly string[] _dateSeparatorArray = new[] { AzureTableStorageLoggingOptions.TableNameSeparator };
	private static readonly string _cacheKeyPrefix = typeof(AzureTableStorageLogManager).FullName;
	private static readonly IReadOnlyCollection<string> _metadataFilters = new[] { nameof(LogEntryMetaData.EventTimeStamp), nameof(LogEntryMetaData.Level) };
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
	/// Gets the table service client.
	/// </summary>
	protected TableServiceClient TableServiceClient { get; }

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
		TableServiceClient = new TableServiceClient(options.AzureStorageConnectionString);
		MemoryCache = memoryCache;
		DistributedCache = distributedCache;

		TableListCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes) };
		LogEntryMetaDataCacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes), Priority = CacheItemPriority.Low };
	}
	#endregion

	#region IAzureTableStorageLogManager Members
	/// <inheritdoc />
	public Task<(IReadOnlyCollection<AzureTableStorageLogDataSource> items, int totalCount)> FindAllDataSourceByOptionsAsync(AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(options);

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

			IReadOnlyCollection<AzureTableStorageLogDataSource> lstSortedFilteredItem = options.PageSize > 0
				? lstDataSource.Skip(itemsToSkip).Take(options.PageSize).ToArray()
				: lstDataSource.ToArray();

			return Task.FromResult((lstSortedFilteredItem, totalCount));
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { options }))
		{
			throw new AzureTableStorageLogManagementException("There was a problem accessing the log data sources.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<(IReadOnlyCollection<AzureTableStorageLogTable> items, int totalCount)> FindAllLogTableByTablePrefixAndOptionsAsync(string tablePrefix, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tablePrefix);
		Guard.IsNotNull(options);

		try
		{
			//Using the DistributedCache to store the table models so that changes are reflected globally
			(IEnumerable<AzureTableStorageLogTable>? cacheItem, UmbrellaDistributedCacheException? exception) = await DistributedCache.GetOrCreateAsync(GenerateCacheKey(tablePrefix), async () =>
			{
				cancellationToken.ThrowIfCancellationRequested();

				var hsResult = new HashSet<TableItem>(_cloudTableEqualityComparer);

				await foreach (var tableItem in TableServiceClient.QueryAsync(x => x.Name.StartsWith(tablePrefix), 100, cancellationToken).ConfigureAwait(false))
				{
					if (tableItem is not null)
						_ = hsResult.Add(tableItem);
				}

				if (hsResult.Count is 0)
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
	public async Task<(AzureTableStorageLogAppenderType? appenderType, IReadOnlyCollection<TEntity> items, int totalCount)> FindAllTableEntityByOptionsAsync<TEntity>(string tablePrefix, string tableName, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
		where TEntity : class, ITableEntity, new()
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tablePrefix);
		Guard.IsNotNullOrWhiteSpace(tableName);
		Guard.IsNotNull(options);

		try
		{
			// Validate the tablePrefix
			var dataSource = LogManagementOptions.DataSources.Find(x => Normalize(x.TablePrefix) == Normalize(tablePrefix));

			if (dataSource is null)
				return (null, new List<TEntity>(), 0);

			TableClient table = TableServiceClient.GetTableClient(tableName);

			bool cacheFirstBuild = false;
			string cacheKey = GenerateCacheKey(tableName);

			//Try and get existing cache items - if there are none build the cache
			List<LogEntryMetaData> lstMetaData = await MemoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
			{
				cacheFirstBuild = true;

				_ = cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

				var lstResult = new List<LogEntryMetaData>();

				await foreach (var item in table.QueryAsync<LogEntryMetaData>(maxPerPage: 100, select: _metadataFilters, cancellationToken: cancellationToken).ConfigureAwait(false))
				{
					lstResult.Add(item);
				}

				return lstResult;
			}).ConfigureAwait(false);

			if (!cacheFirstBuild)
			{
				// Check ATS for any new rows added since the cache was populated but only if
				// we haven't just populated the cache in this request.
				// Get the last item in the cache
				LogEntryMetaData? entry = lstMetaData.LastOrDefault();

				if (entry is not null)
				{
					var lstNewEntries = new List<LogEntryMetaData>();

					await foreach (var item in table.QueryAsync<LogEntryMetaData>(filter: x => x.PartitionKey.CompareTo(entry.PartitionKey) > 0 && x.RowKey.CompareTo(entry.RowKey) > 0, maxPerPage: 100, select: _metadataFilters, cancellationToken: cancellationToken).ConfigureAwait(false))
					{
						lstNewEntries.Add(item);
					}

					//Append the new entries to the current list - these will automatically end up in the cache.
					lstMetaData.AddRange(lstNewEntries);
				}
			}

			int totalCount = lstMetaData.Count();

			// Apply sorting - always default
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

			// Apply pagination
			if (options.PageSize > 0)
			{
				int itemsToSkip = (options.PageNumber - 1) * options.PageSize;
				results = results.Skip(itemsToSkip).Take(options.PageSize);
			}

			// Now we have the paginated / sorted / filtered metadata from the cache
			// we need to retrieve the full items from ATS. Seemingly you can't do multiple
			// point lookups in a batch so each item will have to be retrieved manually.
			IReadOnlyCollection<Task<Response<TEntity>>> lstResponse = results.Select(x => table.GetEntityAsync<TEntity>(x.PartitionKey, x.RowKey, cancellationToken: cancellationToken)).ToArray();

			_ = await Task.WhenAll(lstResponse).ConfigureAwait(false);

			var items = lstResponse.Select(x => x.Result.Value).Where(x => x is not null).ToArray();

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
			TableClient table = TableServiceClient.GetTableClient(tableName);

			using Response response = await table.DeleteAsync(cancellationToken).ConfigureAwait(false);

			if (response.Status is 404)
				return AzureTableStorageLogDeleteOperationResult.NotFound;

			// Remove the table from the cached list - potential for a race condition to mess with this but should be low probability.
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

			// Also need to remove the log entries from the Memory Cache
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