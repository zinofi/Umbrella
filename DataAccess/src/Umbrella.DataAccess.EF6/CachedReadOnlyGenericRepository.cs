using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Caching;
using System.Threading;

namespace Umbrella.DataAccess.EF6
{
    public abstract class CachedReadOnlyGenericRepository<TEntity, TDbContext> : CachedReadOnlyGenericRepository<TEntity, TDbContext, int>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
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
        where TDbContext : DbContext
        where TEntityKey : IEquatable<TEntityKey>
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(CachedReadOnlyGenericRepository<TEntity, TDbContext, TEntityKey>).FullName;
        private static readonly string s_AllCacheKey = $"{s_CacheKeyPrefix}:All";
        private static readonly string s_AllCountCacheKey = $"{s_CacheKeyPrefix}:AllCount";
        #endregion

        #region Private Members
        private readonly IDistributedCache m_Cache;
        private readonly string m_CacheKeyPrefix;
        #endregion

        #region Protected Properties
        protected IDistributedCache Cache => m_Cache;
        #endregion

        #region Constructors
        public CachedReadOnlyGenericRepository(TDbContext dbContext, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer, IDistributedCache cache)
            : base(dbContext, logger, lookupNormalizer)
        {
            m_Cache = cache;

            //For items we are loading to be cached lazy loading must be disabled so that entities can be serialized without
            //their children also being automatically serialized.
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }
        #endregion

        #region Overridden Methods
        public override List<TEntity> FindAll(bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(trackChanges, map);

                return m_Cache.GetOrSetAsJsonString(s_AllCacheKey, () =>
                {
                    List<TEntity> lstEntity = Items.AsNoTracking().ToList();

                    //Also add cache entries for each entity for fast lookup
                    foreach (TEntity entity in lstEntity)
                    {
                        string key = GenerateCacheKey(entity.Id);

                        m_Cache.SetAsJsonString(key, entity);
                    }

                    return lstEntity;
                });
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EnsureValidArguments(trackChanges, map);

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindAll());
        }

        public override TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(trackChanges, map);

                string key = GenerateCacheKey(id);

                return m_Cache.GetOrSetAsJsonString(key, () =>
                {
                    TEntity entity = Items.AsNoTracking().SingleOrDefault(x => x.Id.Equals(id));

                    if(entity != null)
                    {
                        //We need to ensure that the All items cache is cleared to force it to be repopulated
                        //There is no clear way to ensure we don't have concurrency issues when running in a multi-server
                        //environment otherwise.
                        m_Cache.Remove(s_AllCacheKey);
                        m_Cache.Remove(s_AllCountCacheKey);
                    }

                    return entity;
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { id }))
            {
                throw;
            }
        }

        public override Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EnsureValidArguments(trackChanges, map);

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindById(id));
        }

        public override List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(trackChanges, map);

                return ids.Select(x => FindById(x)).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public override Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindAllByIdList(ids));
        }

        public override int FindTotalCount()
        {
            try
            {
                return m_Cache.GetOrSetAsJsonString(s_AllCountCacheKey, () => FindAll().Count);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindTotalCount());
        }
        #endregion

        #region Private Methods
        private string GenerateCacheKey(TEntityKey id) => $"{s_CacheKeyPrefix}:{id}";

        private void EnsureValidArguments(bool trackChanges, IncludeMap<TEntity> map)
        {
            if (trackChanges)
                throw new ArgumentException("Change tracking cannot be enabled for cached entities.", nameof(trackChanges));

            if (map != null)
                throw new ArgumentException("An include map cannot be used for cached entities.", nameof(map));
        }
        #endregion
    }
}