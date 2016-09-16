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
using System.Data.Entity.Infrastructure;

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
        private readonly bool m_InitialLazyLoadingStatus;
        private readonly bool m_InitialProxyCreationStatus;
        #endregion

        #region Protected Properties
        protected IDistributedCache Cache => m_Cache;
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

                DisableLazyLoadingAndProxying();

                List<TEntity> lstEntity = m_Cache.GetOrSetAsJsonString(s_AllCacheKey, () =>
                {
                    List<TEntity> lstEntityFromDb = Items.AsNoTracking().ToList();

                    //Also add cache entries for each entity for fast lookup
                    foreach (TEntity entity in lstEntityFromDb)
                    {
                        string key = GenerateCacheKey(entity.Id);

                        m_Cache.SetAsJsonString(key, entity);
                    }

                    return lstEntityFromDb;
                });

                RestoreLazyLoadingAndProxying();

                if(trackChanges)
                {
                    foreach(TEntity entity in lstEntity)
                    {
                        Context.Entry(entity).State = EntityState.Unchanged;
                    }
                }

                return lstEntity;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EnsureValidArguments(map);

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindAll(trackChanges));
        }

        public override TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                string key = GenerateCacheKey(id);

                DisableLazyLoadingAndProxying();

                TEntity entity = m_Cache.GetOrSetAsJsonString(key, () =>
                {
                    TEntity entityFromDb = Items.AsNoTracking().SingleOrDefault(x => x.Id.Equals(id));

                    if (entityFromDb != null)
                    {
                        //We need to ensure that the All items cache is cleared to force it to be repopulated
                        //There is no clear way to ensure we don't have concurrency issues when running in a multi-server
                        //environment otherwise.
                        m_Cache.Remove(s_AllCacheKey);
                        m_Cache.Remove(s_AllCountCacheKey);
                    }

                    return entityFromDb;
                });

                RestoreLazyLoadingAndProxying();

                if (trackChanges)
                        Context.Entry(entity).State = EntityState.Unchanged;

                return entity;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { id }))
            {
                throw;
            }
        }

        public override Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EnsureValidArguments(map);

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindById(id, trackChanges));
        }

        public override List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                EnsureValidArguments(map);

                return ids.Select(x => FindById(x, trackChanges)).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public override Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            EnsureValidArguments(map);

            //TODO: Need to re-implement this using the proper async / await pattern by replicating the code from the synchronous method.
            return Task.FromResult(FindAllByIdList(ids, trackChanges));
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