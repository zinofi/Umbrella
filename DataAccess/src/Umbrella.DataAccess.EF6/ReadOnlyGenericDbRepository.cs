using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.EF.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
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
			ILookupNormalizer lookupNormalizer)
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
		protected ILookupNormalizer LookupNormalizer { get; }
		protected IQueryable<TEntity> Items => Context.Set<TEntity>();
		#endregion

		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer)
		{
			Context = dbContext;
			Log = logger;
			LookupNormalizer = lookupNormalizer;
		}
		#endregion

		#region IReadOnlyGenericRepository Members
		public virtual async Task<IReadOnlyCollection<TEntity>> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				if (filterExpressions != null)
					throw new NotSupportedException("Filtering is not currently supported.");

				return await Items
					.ApplySortExpressions(sortExpressions, new SortExpression<TEntity>(x => x.Id, SortDirection.Ascending))
					.ApplyPagination(pageNumber, pageSize)
					.TrackChanges(trackChanges)
					.IncludeMap(map)
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, trackChanges, map, sortExpressions, filterExpressions, filterExpressionCombinator }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items using the specified parameters.", exc);
			}
		}

		public virtual async Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(ids, nameof(ids));

			try
			{
				if (filterExpressions != null)
					throw new NotSupportedException("Filtering is not currently supported.");

				return await Items
					.ApplySortExpressions(sortExpressions, new SortExpression<TEntity>(x => x.Id, SortDirection.Ascending))
					.TrackChanges(trackChanges)
					.IncludeMap(map)
					.Where(x => ids.Contains(x.Id))
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ids, trackChanges, map, sortExpressions, filterExpressions, filterExpressionCombinator }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items with the specified ids.", exc);
			}
		}

		public virtual async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, trackChanges, map }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the item with the specified id.", exc);
			}
		}

		public virtual async Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Items.CountAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the count of all items.", exc);
			}
		}
		#endregion
	}
}