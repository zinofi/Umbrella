using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.EF.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.EF6
{
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext> : ReadOnlyGenericDbRepository<TEntity, TDbContext, int>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			IDataAccessLookupNormalizer lookupNormalizer)
			: base(dbContext, logger, lookupNormalizer)
		{
		}
		#endregion
	}

	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TEntityKey> : IReadOnlyGenericDbRepository<TEntity, TEntityKey>
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
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			IDataAccessLookupNormalizer lookupNormalizer)
		{
			Context = dbContext;
			Log = logger;
			LookupNormalizer = lookupNormalizer;
		}
		#endregion

		#region IReadOnlyGenericRepository Members
		public virtual async Task<IReadOnlyCollection<TEntity>> FindAllAsync(CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Items.TrackChanges(trackChanges).IncludeMap(map).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}

		public virtual async Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(ids, nameof(ids));

			try
			{
				return await Items.TrackChanges(trackChanges).IncludeMap(map).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, ids))
			{
				throw;
			}
		}

		public virtual async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, trackChanges }))
			{
				throw;
			}
		}

		public virtual async Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Items.CountAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}
		#endregion
	}
}