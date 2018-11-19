using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;
using Umbrella.Utilities;
using Umbrella.Utilities.Comparers;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Caching;

namespace Umbrella.Extensions.Logging.Azure.Management
{
    public class AzureTableStorageLogManager : IAzureTableStorageLogManager
    {
        #region Private Inner Classes
        private class LogEntryMetaData
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTime EventTimeStamp { get; set; }
            public string Level { get; set; }
        }
        #endregion

        #region Private Members
        private static readonly Dictionary<string, string> s_NormalizedDataSourceKeyDictionary = new Dictionary<string, string>
        {
            [nameof(AzureTableStorageLogDataSource.AppenderType)] = Normalize(nameof(AzureTableStorageLogDataSource.AppenderType)),
            [nameof(AzureTableStorageLogDataSource.CategoryName)] = Normalize(nameof(AzureTableStorageLogDataSource.CategoryName)),
            [nameof(AzureTableStorageLogDataSource.TablePrefix)] = Normalize(nameof(AzureTableStorageLogDataSource.TablePrefix))
        };
        private static readonly Dictionary<string, string> s_NormalizedTableModelKeyDictionary = new Dictionary<string, string>
        {
            [nameof(AzureTableStorageLogTable.Date)] = Normalize(nameof(AzureTableStorageLogTable.Date)),
            [nameof(AzureTableStorageLogTable.Name)] = Normalize(nameof(AzureTableStorageLogTable.Name))
        };
        private static readonly Dictionary<string, string> s_NormalizedLogEntryMetaDataKeyDictionary = new Dictionary<string, string>
        {
            [nameof(LogEntryMetaData.EventTimeStamp)] = Normalize(nameof(LogEntryMetaData.EventTimeStamp)),
            [nameof(LogEntryMetaData.Level)] = Normalize(nameof(LogEntryMetaData.Level)),
        };
        private static readonly GenericEqualityComparer<CloudTable, string> s_CloudTableEqualityComparer = new GenericEqualityComparer<CloudTable, string>(x => x.Name);
        private static readonly string[] s_DateSeparatorArray = new[] { AzureTableStorageLoggingOptions.TableNameSeparator };
        private static readonly string s_CacheKeyPrefix = typeof(AzureTableStorageLogManager).FullName;
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected AzureTableStorageLogManagementOptions LogManagementOptions { get; }
        protected CloudStorageAccount StorageAccount { get; }
        protected IMemoryCache MemoryCache { get; }
        protected IDistributedCache DistributedCache { get; }
        protected DistributedCacheEntryOptions TableListCacheEntryOptions { get; }
        protected MemoryCacheEntryOptions LogEntryMetaDataCacheEntryOptions { get; }
        #endregion

        #region Constructors
        public AzureTableStorageLogManager(ILogger<AzureTableStorageLogManager> logger,
            IOptions<AzureTableStorageLogManagementOptions> managementOptions,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache)
        {
            var options = managementOptions.Value;

            Guard.ArgumentNotNullOrWhiteSpace(options.AzureStorageConnectionString, nameof(options.AzureStorageConnectionString));

            if (options.CacheLifetimeMinutes < 0)
                throw new AzureTableStorageLogManagementException($"The value of {nameof(options.CacheLifetimeMinutes)} cannot be less than zero");

            Log = logger;
            LogManagementOptions = options;
            StorageAccount = CloudStorageAccount.Parse(options.AzureStorageConnectionString);
            MemoryCache = memoryCache;
            DistributedCache = distributedCache;

            TableListCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes) };
            LogEntryMetaDataCacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(options.CacheLifetimeMinutes), Priority = CacheItemPriority.Low };
        }
        #endregion

        #region IAzureTableStorageLogManager Members
        public Task<(List<AzureTableStorageLogDataSource> Items, int TotalCount)> FindAllDataSourceByOptionsAsync(AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(options, nameof(options));

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
                            if (key == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.AppenderType)])
                                lstDataSource = lstDataSource.Where(x => x.AppenderType == value.ToEnum<AzureTableStorageLogAppenderType>());
                            else if (key == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.CategoryName)])
                                lstDataSource = lstDataSource.Where(x => Normalize(x.CategoryName).Contains(value));
                            else if (key == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.TablePrefix)])
                                lstDataSource = lstDataSource.Where(x => Normalize(x.TablePrefix).Contains(value));
                        }
                    }
                }

                int totalCount = lstDataSource.Count();

                //Apply sorting - always default
                string sortBy = options.SortProperty;

                if (string.IsNullOrWhiteSpace(sortBy))
                    sortBy = nameof(AzureTableStorageLogDataSource.CategoryName);

                sortBy = Normalize(sortBy);

                if (sortBy == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.AppenderType)])
                    lstDataSource = lstDataSource.OrderBySortDirection(x => x.AppenderType, options.SortDirection);
                else if (sortBy == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.CategoryName)])
                    lstDataSource = lstDataSource.OrderBySortDirection(x => x.CategoryName, options.SortDirection);
                else if (sortBy == s_NormalizedDataSourceKeyDictionary[nameof(AzureTableStorageLogDataSource.TablePrefix)])
                    lstDataSource = lstDataSource.OrderBySortDirection(x => x.TablePrefix, options.SortDirection);
                else
                    lstDataSource = lstDataSource.OrderByDescending(x => x.CategoryName);

                //Apply pagination
                int itemsToSkip = (options.PageNumber - 1) * options.PageSize;

                List<AzureTableStorageLogDataSource> lstSortedFilteredItem = options.PageSize > 0
                    ? lstDataSource.Skip(itemsToSkip).Take(options.PageSize).ToList()
                    : lstDataSource.ToList();

                return Task.FromResult((lstSortedFilteredItem, totalCount));
            }
            catch (Exception exc) when (Log.WriteError(exc, new { options }, returnValue: true))
            {
                throw new AzureTableStorageLogManagementException("There was a problem accessing the log data sources.", exc);
            }
        }

        public async Task<(List<AzureTableStorageLogTable> Items, int TotalCount)> FindAllLogTableByTablePrefixAndOptionsAsync(string tablePrefix, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));
            Guard.ArgumentNotNull(options, nameof(options));

            try
            {
                //Using the DistributedCache to store the table models so that changes are reflected globally
                (IEnumerable<AzureTableStorageLogTable> cacheItem, UmbrellaDistributedCacheException exception) = await DistributedCache.GetOrCreateAsync(GenerateCacheKey(tablePrefix), async innerToken =>
                {
                    CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

                    TableContinuationToken continuationToken = null;
                    TableResultSegment resultSegment = null;

                    HashSet<CloudTable> hsResult = new HashSet<CloudTable>(s_CloudTableEqualityComparer);

                    do
                    {
                        resultSegment = await tableClient.ListTablesSegmentedAsync(tablePrefix, continuationToken, innerToken).ConfigureAwait(false);

                        foreach (CloudTable table in resultSegment.Results)
                        {
                            if (table != null)
                                hsResult.Add(table);
                        }

                        continuationToken = resultSegment.ContinuationToken;
                    }
                    while (continuationToken != null);

                    if (hsResult.Count == 0)
                        return null;

                    var cbTableModel = new ConcurrentBag<AzureTableStorageLogTable>();

                    hsResult.AsParallel().ForAll(x =>
                    {
                        //The date is stored as part of the table name in the format {tablePrefix}xxxxxx{yyyy}x{mm}x{dd}, e.g. CostsBudgITPortalServerxxxxxx2016-09-27
                        string[] strDateParts = x.Name.Split(s_DateSeparatorArray, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1)?.Split('x');

                        if (strDateParts.Length == 3)
                        {
                            int year = int.Parse(strDateParts[0]);
                            int month = int.Parse(strDateParts[1]);
                            int day = int.Parse(strDateParts[2]);

                            DateTime dtDate = new DateTime(year, month, day);

                            var tableModel = new AzureTableStorageLogTable
                            {
                                Date = dtDate,
                                Name = x.Name
                            };

                            cbTableModel.Add(tableModel);
                        }
                    });

                    return cbTableModel.ToList();
                }, () => TableListCacheEntryOptions).ConfigureAwait(false);

                int totalCount = cacheItem?.Count() ?? 0;

                if (totalCount == 0)
                    return (new List<AzureTableStorageLogTable>(), 0);

                //Apply sorting - always default
                string sortBy = options.SortProperty;

                if (string.IsNullOrWhiteSpace(sortBy))
                    sortBy = nameof(AzureTableStorageLogTable.Date);

                sortBy = Normalize(sortBy);

                if (sortBy == s_NormalizedTableModelKeyDictionary[nameof(AzureTableStorageLogTable.Date)])
                    cacheItem = cacheItem.OrderBySortDirection(x => x.Date, options.SortDirection);
                else if (sortBy == s_NormalizedTableModelKeyDictionary[nameof(AzureTableStorageLogTable.Name)])
                    cacheItem = cacheItem.OrderBySortDirection(x => x.Name, options.SortDirection);
                else
                    cacheItem = cacheItem.OrderByDescending(x => x.Date);

                //Apply pagination
                int itemsToSkip = (options.PageNumber - 1) * options.PageSize;

                List<AzureTableStorageLogTable> lstSortedFilteredItem = options.PageSize > 0
                    ? cacheItem.Skip(itemsToSkip).Take(options.PageSize).ToList()
                    : cacheItem.ToList();

                return (lstSortedFilteredItem, totalCount);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { tablePrefix, options }, returnValue: true))
            {
                throw new AzureTableStorageLogManagementException($"There was a problem accessing the log table table data for table prefix {tablePrefix}", exc);
            }
        }

        public async Task<(AzureTableStorageLogAppenderType? AppenderType, List<TableEntity> Items, int TotalCount)> FindAllTableEntityByOptionsAsync(string tablePrefix, string tableName, AzureTableStorageLogSearchOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));
            Guard.ArgumentNotNullOrWhiteSpace(tableName, nameof(tableName));
            Guard.ArgumentNotNull(options, nameof(options));

            try
            {
                //Validate the tablePrefix
                var dataSource = LogManagementOptions.DataSources.Find(x => Normalize(x.TablePrefix) == Normalize(tablePrefix));

                if (dataSource == null)
                    return (null, new List<TableEntity>(), 0);

                CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(tableName);

                //Build the query we will use to populate the metadata cache
                TableQuery<DynamicTableEntity> cacheQuery = new TableQuery<DynamicTableEntity>().Select(new[] { nameof(LogEntryMetaData.EventTimeStamp), nameof(LogEntryMetaData.Level) });

                bool cacheFirstBuild = false;
                string cacheKey = GenerateCacheKey(tableName);
                TableContinuationToken continuationToken = null;
                TableQuerySegment<DynamicTableEntity> querySegment = null;

                //Try and get existing cache items - if there are none build the cache
                List<LogEntryMetaData> lstMetaData = await MemoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
                {
                    cacheFirstBuild = true;

                    cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

                    List<LogEntryMetaData> lstResult = new List<LogEntryMetaData>();

                    do
                    {
                        querySegment = await table.ExecuteQuerySegmentedAsync(cacheQuery, continuationToken, cancellationToken).ConfigureAwait(false);

                        foreach (var entity in querySegment.Results)
                        {
                            LogEntryMetaData result = new LogEntryMetaData
                            {
                                PartitionKey = entity.PartitionKey,
                                RowKey = entity.RowKey,
                                EventTimeStamp = entity[nameof(LogEntryMetaData.EventTimeStamp)].DateTime.Value,
                                Level = entity[nameof(LogEntryMetaData.Level)].StringValue
                            };

                            lstResult.Add(result);
                        }

                        continuationToken = querySegment.ContinuationToken;
                    }
                    while (continuationToken != null);

                    return lstResult;
                }).ConfigureAwait(false);

                if (!cacheFirstBuild)
                {
                    //Check ATS for any new rows added since the cache was populated but only if
                    //we haven't just populated the cache in this request.
                    //Get the last item in the cache
                    LogEntryMetaData entry = lstMetaData?.LastOrDefault();

                    if (entry != null)
                    {
                        List<LogEntryMetaData> lstNewEntries = new List<LogEntryMetaData>();

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
                                LogEntryMetaData result = new LogEntryMetaData
                                {
                                    PartitionKey = entity.PartitionKey,
                                    RowKey = entity.RowKey,
                                    EventTimeStamp = entity[nameof(LogEntryMetaData.EventTimeStamp)].DateTime.Value,
                                    Level = entity[nameof(LogEntryMetaData.Level)].StringValue
                                };

                                lstNewEntries.Add(result);
                            }

                            continuationToken = querySegment.ContinuationToken;
                        }
                        while (continuationToken != null);

                        //Append the new entries to the current list - these will automatically end up in the cache.
                        lstMetaData.AddRange(lstNewEntries);
                    }
                }

                int totalCount = lstMetaData.Count();

                //Apply sorting - always default
                string sortBy = options.SortProperty;

                if (string.IsNullOrWhiteSpace(sortBy))
                    sortBy = nameof(LogEntryMetaData.EventTimeStamp);

                sortBy = Normalize(sortBy);

                IEnumerable<LogEntryMetaData> results = lstMetaData.ToList();

                if (sortBy == s_NormalizedLogEntryMetaDataKeyDictionary[nameof(LogEntryMetaData.EventTimeStamp)])
                    results = results.OrderBySortDirection(x => x.EventTimeStamp, options.SortDirection);
                else if (sortBy == s_NormalizedLogEntryMetaDataKeyDictionary[nameof(LogEntryMetaData.Level)])
                    results = results.OrderBySortDirection(x => x.Level, options.SortDirection);
                else
                    results = results.OrderByDescending(x => x.EventTimeStamp);

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
                    : (Func<string, string, List<string>, TableOperation>)TableOperation.Retrieve<AzureLoggingServerEventEntity>;

                var opResults = await Task.WhenAll(results.Select(x => table.ExecuteAsync(tableOperation(x.PartitionKey, x.RowKey, null)))).ConfigureAwait(false);

                var items = opResults.Select(x => x.Result).Cast<TableEntity>().ToList();

                return (dataSource.AppenderType, items, totalCount);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { tablePrefix, tableName, options }, returnValue: true))
            {
                throw new AzureTableStorageLogManagementException($"There was a problem accessing the log table table data for table prefix {tablePrefix} and table name {tableName}", exc);
            }
        }

        public async Task<AzureTableStorageLogDeleteOperationResult> DeleteTableByNameAsync(string tableName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(tableName, nameof(tableName));

            try
            {
                CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(tableName);

                bool exists = await table.ExistsAsync(cancellationToken).ConfigureAwait(false);

                if (!exists)
                    return AzureTableStorageLogDeleteOperationResult.NotFound;

                //Using the IfExists variant here in case a race condition has resulted in the table already having been deleted by another user.
                await table.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);

                //Remove the table from the cached list - potential for a race condition to mess with this but should be low probability.
                string tablePrefix = tableName.Split(s_DateSeparatorArray, StringSplitOptions.RemoveEmptyEntries)[0];
                string listCacheKey = GenerateCacheKey(tablePrefix);

                var lstTableModel = await DistributedCache.GetAsync<List<AzureTableStorageLogTable>>(listCacheKey, cancellationToken).ConfigureAwait(false);

                if (lstTableModel != null)
                {
                    var itemToRemove = lstTableModel.Find(x => x.Name == tableName);

                    if (itemToRemove != null)
                    {
                        lstTableModel.Remove(itemToRemove);
                        await DistributedCache.SetAsync(listCacheKey, lstTableModel, () => TableListCacheEntryOptions, cancellationToken).ConfigureAwait(false);
                    }
                }

                //Also need to remove the log entries from the Memory Cache
                MemoryCache.Remove(GenerateCacheKey(tableName));

                return AzureTableStorageLogDeleteOperationResult.Success;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { tableName }, returnValue: true))
            {
                throw new AzureTableStorageLogManagementException($"There has been a problem deleting the table with name {tableName}", exc);
            }
        }

        public async Task ClearTableNameCacheAsync(string tablePrefix, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(tablePrefix, nameof(tablePrefix));

            try
            {
                string cacheKey = GenerateCacheKey(tablePrefix);

                await DistributedCache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { tablePrefix }, returnValue: true))
            {
                throw new AzureTableStorageLogManagementException($"There has been a problem clearing the cache for the table prefix {tablePrefix}", exc);
            }
        }
        #endregion

        #region Private Static Methods
        private static string Normalize(string value) => value?.Trim()?.ToUpperInvariant()?.Normalize();
        #endregion

        #region Private Methods
        private string GenerateCacheKey(string key) => Normalize($"{s_CacheKeyPrefix}:{key}");

        private async Task<List<TTableEntity>> BuildListingModelAsync<TTableEntity>(AzureTableStorageLogDataSource dataSource, CloudTable table, int totalCount, IEnumerable<LogEntryMetaData> lstSortedFilteredItem)
                where TTableEntity : ITableEntity
        {
            var results = await Task.WhenAll(lstSortedFilteredItem.Select(x => table.ExecuteAsync(TableOperation.Retrieve<TTableEntity>(x.PartitionKey, x.RowKey))));

            var items = results.Select(x => x.Result).Cast<TTableEntity>();

            return items.ToList();
        }
        #endregion
    }
}