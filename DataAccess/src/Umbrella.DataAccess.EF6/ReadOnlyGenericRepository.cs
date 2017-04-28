using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.EF6
{
    public abstract class ReadOnlyGenericRepository<TEntity, TDbContext> : ReadOnlyGenericRepository<TEntity, TDbContext, int>
        where TEntity : class, IEntity<int>
        where TDbContext : UmbrellaDbContext
    {
        #region Constructors
        public ReadOnlyGenericRepository(TDbContext dbContext, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
            : base(dbContext, logger, lookupNormalizer)
        {
        } 
        #endregion
    }

    public abstract class ReadOnlyGenericRepository<TEntity, TDbContext, TEntityKey> : IReadOnlyGenericRepository<TEntity, TEntityKey>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : UmbrellaDbContext
        where TEntityKey : IEquatable<TEntityKey>
    {
        #region Protected Properties
        protected TDbContext Context { get; }
        protected ILogger Log { get; }
        protected IDataAccessLookupNormalizer LookupNormalizer { get; }
        protected IQueryable<TEntity> Items => Context.Set<TEntity>();
        #endregion

        #region Constructors
        public ReadOnlyGenericRepository(TDbContext dbContext, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
        {
            Context = dbContext;
            Log = logger;
            LookupNormalizer = lookupNormalizer;
        }
        #endregion

        #region IReadOnlyGenericRepository Members
        public virtual List<TEntity> FindAll(bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                return Items.TrackChanges(trackChanges).IncludeMap(map).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.TrackChanges(trackChanges).IncludeMap(map).ToListAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                Guard.ArgumentNotNull(ids, nameof(ids));

                return Items.TrackChanges(trackChanges).IncludeMap(map).Where(x => ids.Contains(x.Id)).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guard.ArgumentNotNull(ids, nameof(ids));

                return Items.TrackChanges(trackChanges).IncludeMap(map).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public virtual TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                return Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefault(x => x.Id.Equals(id));
            }
            catch (Exception exc) when (Log.WriteError(exc, id))
            {
                throw;
            }
        }

        public virtual Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc, id))
            {
                throw;
            }
        }

        public virtual int FindTotalCount()
        {
            try
            {
                return Items.Count();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.CountAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}