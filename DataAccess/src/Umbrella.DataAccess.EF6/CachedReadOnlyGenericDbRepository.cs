using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.EF.Abstractions;
using Umbrella.Utilities.Caching;

namespace Umbrella.DataAccess.EF6
{
	public abstract class CachedReadOnlyGenericDbRepository<TEntity, TDbContext> : CachedReadOnlyGenericDbRepository<TEntity, TDbContext, int>, ICachedReadOnlyGenericDbRepository<TEntity>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		#region Constructors
		public CachedReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			IDataAccessLookupNormalizer lookupNormalizer,
			IDistributedCache cache)
			: base(dbContext, logger, lookupNormalizer, cache)
		{
		}
		#endregion
	}

	public abstract class CachedReadOnlyGenericDbRepository<TEntity, TDbContext, TEntityKey> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TEntityKey>, ICachedReadOnlyGenericDbRepository<TEntity, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TEntityKey : IEquatable<TEntityKey>
	{
		#region Private Members
		private readonly bool m_InitialLazyLoadingStatus;
		private readonly bool m_InitialProxyCreationStatus;
		#endregion

		#region Protected Static Properties
		protected static string CacheKeyPrefix => typeof(CachedReadOnlyGenericDbRepository<TEntity, TDbContext, TEntityKey>).FullName;
		protected static string AllCacheKey => $"{CacheKeyPrefix}:All";
		protected static string AllCountCacheKey => $"{CacheKeyPrefix}:AllCount";
		#endregion

		#region Protected Properties
		protected IDistributedCache Cache { get; }
		protected virtual DistributedCacheEntryOptions CacheOptions { get; }
		protected virtual JsonSerializerSettings JsonSettings { get; } // TODO: Remove and abstract away if required.
		protected abstract string EntitySetName { get; }
		#endregion

		#region Constructors
		public CachedReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			IDataAccessLookupNormalizer lookupNormalizer,
			IDistributedCache cache)
			: base(dbContext, logger, lookupNormalizer)
		{
			Cache = cache;
			m_InitialLazyLoadingStatus = Context.Configuration.LazyLoadingEnabled;
			m_InitialProxyCreationStatus = Context.Configuration.ProxyCreationEnabled;
		}
		#endregion

		#region Overridden Methods
		public override async Task<IReadOnlyCollection<TEntity>> FindAllAsync(CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureValidArguments(map);

			try
			{
				var lstCacheEntry = await CacheFindAllAsync(cancellationToken, trackChanges, map).ConfigureAwait(false);

				return lstCacheEntry.Select(x => x.Item).ToList();
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}

		public override async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				EnsureValidArguments(map);

				var entry = await CacheFindByIdAsync(id, cancellationToken, trackChanges, map).ConfigureAwait(false);

				return entry?.Item;
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}

		public override async Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureValidArguments(map);

			try
			{
				var lstMatches = new List<TEntity>();

				foreach (var id in ids)
				{
					var match = await FindByIdAsync(id, cancellationToken, trackChanges, map).ConfigureAwait(false);

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

		public override async Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var (cacheItem, exception) = await Cache.GetOrCreateAsync(AllCountCacheKey, async () =>
				{
					cancellationToken.ThrowIfCancellationRequested();

					var allItems = await FindAllAsync(cancellationToken).ConfigureAwait(false);

					return allItems.Count;
				}, () => CacheOptions).ConfigureAwait(false);

				return cacheItem;
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}
		#endregion

		#region ICachedReadOnlyGenericDbRepository Members
		public async Task<IReadOnlyCollection<CacheEntry<TEntity>>> CacheFindAllAsync(CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureValidArguments(map);

			try
			{	
				DisableLazyLoadingAndProxying();

				var (cacheItem, exception) = await Cache.GetOrCreateAsync(AllCacheKey, async () =>
				{
					cancellationToken.ThrowIfCancellationRequested();

					List<TEntity> lstEntityFromDb = await Items.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
					var lstCacheEntryFromDb = new List<CacheEntry<TEntity>>();

					//Also add cache entries for each entity for fast lookup
					foreach (TEntity entity in lstEntityFromDb)
					{
						string key = GenerateCacheKey(entity.Id);

						var entry = new CacheEntry<TEntity>(entity);
						await PopulateCacheEntryMetaDataAsync(entry, cancellationToken).ConfigureAwait(false);

						await Cache.SetAsync(key, entry, CacheOptions, cancellationToken).ConfigureAwait(false);
						lstCacheEntryFromDb.Add(entry);
					}

					return lstCacheEntryFromDb;
				}, () => CacheOptions).ConfigureAwait(false);

				RestoreLazyLoadingAndProxying();

				if (trackChanges)
				{
					foreach (var cacheEntry in cacheItem)
					{
						AttachToContext(cacheEntry);
					}
				}

				return cacheItem;
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}

		public async Task<CacheEntry<TEntity>> CacheFindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureValidArguments(map);

			try
			{
				string key = GenerateCacheKey(id);

				DisableLazyLoadingAndProxying();

				var (cacheItem, exception) = await Cache.GetOrCreateAsync(key, async () =>
				{
					cancellationToken.ThrowIfCancellationRequested();

					TEntity entityFromDb = await Items.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken).ConfigureAwait(false);

					if (entityFromDb != null)
					{
						var cacheEntryFromDb = new CacheEntry<TEntity>(entityFromDb);
						await PopulateCacheEntryMetaDataAsync(cacheEntryFromDb, cancellationToken).ConfigureAwait(false);

						//We need to ensure that the All items cache is cleared to force it to be repopulated
						//There is no clear way to ensure we don't have concurrency issues when running in a multi-server
						//environment otherwise.
						await Cache.RemoveAsync(AllCacheKey, cancellationToken).ConfigureAwait(false);
						await Cache.RemoveAsync(AllCountCacheKey, cancellationToken).ConfigureAwait(false);

						return cacheEntryFromDb;
					}

					return null;
				}, () => CacheOptions).ConfigureAwait(false);

				RestoreLazyLoadingAndProxying();

				if (trackChanges && cacheItem != null)
					AttachToContext(cacheItem);

				return cacheItem;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id }))
			{
				throw;
			}
		}
		#endregion

		#region Protected Methods
		protected virtual Task PopulateCacheEntryMetaDataAsync(CacheEntry<TEntity> cacheEntry, CancellationToken cancellationToken = default) => Task.CompletedTask;

		protected void AttachToContext(CacheEntry<TEntity> cacheEntry)
		{
			var entity = cacheEntry.Item;

			ObjectContext objectContext = ((IObjectContextAdapter)Context).ObjectContext;

			bool attach;

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