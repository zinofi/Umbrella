using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Umbrella.Utilities.Caching;
using System.Threading;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using System.Data.Entity.Core.Objects;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EF6
{
    public abstract class CachedReadOnlyGenericRepository<TEntity, TDbContext> : CachedReadOnlyGenericRepository<TEntity, TDbContext, int>
        where TEntity : class, IEntity<int>
        where TDbContext : UmbrellaDbContext
    {
        #region Constructors
        public CachedReadOnlyGenericRepository(TDbContext dbContext, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer, IDistributedCache cache)
            : base(dbContext, logger, lookupNormalizer, cache)
        {
        }
        #endregion
    }

    public abstract class CachedReadOnlyGenericRepository<TEntity, TDbContext, TEntityKey> : ReadOnlyGenericRepository<TEntity, TDbContext, TEntityKey>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : UmbrellaDbContext
        where TEntityKey : IEquatable<TEntityKey>
    {
        #region Private Members
        private readonly IDistributedCache m_Cache;
        private readonly bool m_InitialLazyLoadingStatus;
        private readonly bool m_InitialProxyCreationStatus;
        #endregion

        #region Protected Static Properties
        protected static string CacheKeyPrefix => typeof(CachedReadOnlyGenericRepository<TEntity, TDbContext, TEntityKey>).FullName;
        protected static string AllCacheKey => $"{CacheKeyPrefix}:All";
        protected static string AllCountCacheKey => $"{CacheKeyPrefix}:AllCount";
        #endregion

        #region Protected Properties
        protected IDistributedCache Cache => m_Cache;
        protected virtual DistributedCacheEntryOptions CacheOptions { get; }
        protected virtual JsonSerializerSettings JsonSettings { get; }
        protected abstract string EntitySetName { get; }
        #endregion

        #region Constructors
        public CachedReadOnlyGenericRepository(TDbContext dbContext, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer, IDistributedCache cache)
            : base(dbContext, logger, lookupNormalizer)
        {
            m_Cache = cache;
            m_InitialLazyLoadingStatus = Context.Configuration.LazyLoadingEnabled;
            m_InitialProxyCreationStatus = Context.Configuration.ProxyCreationEnabled;
        }
        #endregion

        #region Overridden Methods
        public override List<TEntity> FindAll(bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                return CacheFindAll(trackChanges, map).Select(x => x.Item).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override async Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                EnsureValidArguments(map);

                var lstCacheEntry = await CacheFindAllAsync(cancellationToken, trackChanges, map);

                return lstCacheEntry.Select(x => x.Item).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                return CacheFindById(id, trackChanges, map)?.Item;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { id }))
            {
                throw;
            }
        }

        public override async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                EnsureValidArguments(map);

                var entry = await CacheFindByIdAsync(id, cancellationToken, trackChanges, map);

                return entry?.Item;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                return ids.Select(x => FindById(x, trackChanges, map)).Where(x => x != null).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids }))
            {
                throw;
            }
        }

        public override async Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                EnsureValidArguments(map);

                List<TEntity> lstMatches = new List<TEntity>();

                foreach (var id in ids)
                {
                    var match = await FindByIdAsync(id, cancellationToken, trackChanges, map);

                    if (match != null)
                        lstMatches.Add(match);
                }

                return lstMatches;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids }))
            {
                throw;
            }
        }

        public override int FindTotalCount()
        {
            try
            {
                return m_Cache.GetOrSetAsJsonString(AllCountCacheKey, () => FindAll().Count);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return m_Cache.GetOrSetAsJsonStringAsync(AllCountCacheKey, async () =>
                {
                    var allItems = await FindAllAsync();

                    return allItems.Count;
                });
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion

        #region Protected Methods
        protected List<CacheEntry<TEntity>> CacheFindAll(bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                DisableLazyLoadingAndProxying();

                List<CacheEntry<TEntity>> lstCacheEntry = m_Cache.GetOrSetAsJsonString(AllCacheKey, () =>
                {
                    List<TEntity> lstEntityFromDb = Items.AsNoTracking().ToList();
                    List<CacheEntry<TEntity>> lstCacheEntryFromDb = new List<CacheEntry<TEntity>>();

                    //Also add cache entries for each entity for fast lookup
                    foreach (TEntity entity in lstEntityFromDb)
                    {
                        string key = GenerateCacheKey(entity.Id);

                        var entry = new CacheEntry<TEntity>(entity);
                        PopulateCacheEntryMetaData(entry);

                        m_Cache.SetAsJsonString(key, entry, CacheOptions, JsonSettings);
                        lstCacheEntryFromDb.Add(entry);
                    }

                    return lstCacheEntryFromDb;
                }, CacheOptions, JsonSettings);

                RestoreLazyLoadingAndProxying();

                if (trackChanges)
                {
                    foreach (var cacheEntry in lstCacheEntry)
                    {
                        AttachToContext(cacheEntry);
                    }
                }

                return lstCacheEntry;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public async Task<List<CacheEntry<TEntity>>> CacheFindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                EnsureValidArguments(map);

                DisableLazyLoadingAndProxying();

                List<CacheEntry<TEntity>> lstCacheEntry = await m_Cache.GetOrSetAsJsonStringAsync(AllCacheKey, async () =>
                {
                    List<TEntity> lstEntityFromDb = await Items.AsNoTracking().ToListAsync();
                    List<CacheEntry<TEntity>> lstCacheEntryFromDb = new List<CacheEntry<TEntity>>();

                    //Also add cache entries for each entity for fast lookup
                    foreach (TEntity entity in lstEntityFromDb)
                    {
                        string key = GenerateCacheKey(entity.Id);

                        var entry = new CacheEntry<TEntity>(entity);
                        await PopulateCacheEntryMetaDataAsync(entry);

                        await m_Cache.SetAsJsonStringAsync(key, entry, CacheOptions, JsonSettings);
                        lstCacheEntryFromDb.Add(entry);
                    }

                    return lstCacheEntryFromDb;
                }, CacheOptions, JsonSettings);

                RestoreLazyLoadingAndProxying();

                if (trackChanges)
                {
                    foreach (var cacheEntry in lstCacheEntry)
                    {
                        AttachToContext(cacheEntry);
                    }
                }

                return lstCacheEntry;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public CacheEntry<TEntity> CacheFindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                string key = GenerateCacheKey(id);

                DisableLazyLoadingAndProxying();

                CacheEntry<TEntity> cacheEntry = m_Cache.GetOrSetAsJsonString(key, () =>
                {
                    TEntity entityFromDb = Items.AsNoTracking().SingleOrDefault(x => x.Id.Equals(id));

                    if (entityFromDb != null)
                    {
                        var cacheEntryFromDb = new CacheEntry<TEntity>(entityFromDb);
                        PopulateCacheEntryMetaData(cacheEntryFromDb);

                        //We need to ensure that the All items cache is cleared to force it to be repopulated
                        //There is no clear way to ensure we don't have concurrency issues when running in a multi-server
                        //environment otherwise.
                        m_Cache.Remove(AllCacheKey);
                        m_Cache.Remove(AllCountCacheKey);

                        return cacheEntryFromDb;
                    }

                    return null;
                }, CacheOptions, JsonSettings);

                RestoreLazyLoadingAndProxying();

                if (trackChanges && cacheEntry != null)
                    AttachToContext(cacheEntry);

                return cacheEntry;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { id }))
            {
                throw;
            }
        }

        public async Task<CacheEntry<TEntity>> CacheFindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                string key = GenerateCacheKey(id);

                DisableLazyLoadingAndProxying();

                CacheEntry<TEntity> cacheEntry = await m_Cache.GetOrSetAsJsonStringAsync(key, async () =>
                {
                    TEntity entityFromDb = await Items.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(id));

                    if (entityFromDb != null)
                    {
                        var cacheEntryFromDb = new CacheEntry<TEntity>(entityFromDb);
                        await PopulateCacheEntryMetaDataAsync(cacheEntryFromDb);

                        //We need to ensure that the All items cache is cleared to force it to be repopulated
                        //There is no clear way to ensure we don't have concurrency issues when running in a multi-server
                        //environment otherwise.
                        await m_Cache.RemoveAsync(AllCacheKey);
                        await m_Cache.RemoveAsync(AllCountCacheKey);

                        return cacheEntryFromDb;
                    }

                    return null;
                }, CacheOptions, JsonSettings);

                RestoreLazyLoadingAndProxying();

                if (trackChanges && cacheEntry != null)
                    AttachToContext(cacheEntry);

                return cacheEntry;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { id }))
            {
                throw;
            }
        }

        protected virtual void PopulateCacheEntryMetaData(CacheEntry<TEntity> cacheEntry)
        {
        }

        protected virtual Task PopulateCacheEntryMetaDataAsync(CacheEntry<TEntity> cacheEntry)
        {
            return Task.CompletedTask;
        }

        protected void AttachToContext(CacheEntry<TEntity> cacheEntry)
        {
            var entity = cacheEntry.Item;

            ObjectContext objectContext = ((IObjectContextAdapter)Context).ObjectContext;

            bool attach = false;

            if (objectContext.ObjectStateManager.TryGetObjectStateEntry(objectContext.CreateEntityKey(EntitySetName, entity), out ObjectStateEntry stateEntry))
            {
                attach = stateEntry.State == EntityState.Detached;
                entity = cacheEntry.Item = (TEntity)stateEntry.Entity;
            }
            else
            {
                attach = true;
            }

            if (attach)
                Context.Entry(entity).State = EntityState.Unchanged;
        }
        #endregion

        #region Private Methods
        private string GenerateCacheKey(TEntityKey id) => $"{CacheKeyPrefix}:{id}";

        private void EnsureValidArguments(IncludeMap<TEntity> map)
        {
            if (map != null)
                throw new ArgumentException("An include map cannot be used for cached entities.", nameof(map));
        }

        private void DisableLazyLoadingAndProxying()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;
        }

        private void RestoreLazyLoadingAndProxying()
        {
            Context.Configuration.LazyLoadingEnabled = m_InitialLazyLoadingStatus;
            Context.Configuration.ProxyCreationEnabled = m_InitialProxyCreationStatus;
        }
        #endregion
    }
}