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
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext> : ReadOnlyGenericDbRepository<TEntity, TDbContext, RepoOptions>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor)
		{
		}
		#endregion
	}

	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
	{
		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor)
		{
		}
		#endregion
	}

	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor)
		{
		}
		#endregion
	}

	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
		where TUserAuditKey : IEquatable<TUserAuditKey>
	{
		#region Protected Static Properties
		protected static TRepoOptions DefaultRepoOptions { get; } = new TRepoOptions();
		#endregion

		#region Protected Properties
		protected TDbContext Context { get; }
		protected ILogger Log { get; }
		protected ILookupNormalizer LookupNormalizer { get; }
		protected IQueryable<TEntity> Items => Context.Set<TEntity>();
		protected TUserAuditKey CurrentUserId => CurrentUserIdAccessor.CurrentUserId;
		protected ICurrentUserIdAccessor<TUserAuditKey> CurrentUserIdAccessor { get; }
		#endregion

		#region Constructors
		public ReadOnlyGenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<TUserAuditKey> currentUserIdAccessor)
		{
			Context = dbContext;
			Log = logger;
			LookupNormalizer = lookupNormalizer;
			CurrentUserIdAccessor = currentUserIdAccessor;
		}
		#endregion

		#region IReadOnlyGenericRepository Members
		public virtual async Task<(IReadOnlyCollection<TEntity> results, int totalCount)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			repoOptions ??= DefaultRepoOptions;

			try
			{
				if (filterExpressions != null)
					throw new NotSupportedException("Filtering is not currently supported.");

				int totalCount = await Items.CountAsync();
				List<TEntity> entities = await Items
					.ApplySortExpressions(sortExpressions, new SortExpression<TEntity>(x => x.Id, SortDirection.Ascending))
					.ApplyPagination(pageNumber, pageSize)
					.TrackChanges(trackChanges)
					.IncludeMap(map)
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);

				entities = await FilterByAccessAsync(entities, false, cancellationToken).ConfigureAwait(false);
				await AfterAllItemsLoadedAsync(entities, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return (entities, totalCount);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, trackChanges, map, sortExpressions = sortExpressions.ToSortExpressionSerializables(), filterExpressions = filterExpressions.ToFilterExpressionSerializables(), filterExpressionCombinator }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items using the specified parameters.", exc);
			}
		}

		public virtual async Task<(IReadOnlyCollection<TEntity> results, int totalCount)> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(ids, nameof(ids));
			repoOptions ??= DefaultRepoOptions;

			try
			{
				if (filterExpressions != null)
					throw new NotSupportedException("Filtering is not currently supported.");

				int totalCount = await Items.CountAsync();
				List<TEntity> entities = await Items
					.ApplySortExpressions(sortExpressions, new SortExpression<TEntity>(x => x.Id, SortDirection.Ascending))
					.TrackChanges(trackChanges)
					.IncludeMap(map)
					.Where(x => ids.Contains(x.Id))
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);

				entities = await FilterByAccessAsync(entities, false, cancellationToken).ConfigureAwait(false);
				await AfterAllItemsLoadedAsync(entities, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return (entities, totalCount);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ids, trackChanges, map, sortExpressions = sortExpressions.ToSortExpressionSerializables(), filterExpressions = filterExpressions.ToFilterExpressionSerializables(), filterExpressionCombinator }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items with the specified ids.", exc);
			}
		}

		public virtual async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			repoOptions ??= DefaultRepoOptions;

			try
			{
				var entity = await Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken).ConfigureAwait(false);

				if (entity == null)
					return null;

				if (!await CanAccessAsync(entity, cancellationToken).ConfigureAwait(false))
					throw new UmbrellaDataAccessForbiddenException();

				await AfterItemLoadedAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return entity;
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

		#region Protected Methods
		protected virtual Task<bool> CanAccessAsync(TEntity entity, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}

		protected async Task<List<TEntity>> FilterByAccessAsync(IEnumerable<TEntity> entities, bool throwAccessException, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var lstFilteredEntity = new List<TEntity>();

			foreach (TEntity entity in entities)
			{
				if (!await CanAccessAsync(entity, cancellationToken).ConfigureAwait(false))
				{
					Log.WriteWarning(state: new { Type = entity.GetType().FullName, entity.Id }, message: "The specified item failed the access check. This should not happen.");

					if (throwAccessException)
						throw new UmbrellaDataAccessForbiddenException();

					lstFilteredEntity.Add(entity);
				}
			}

			return lstFilteredEntity;
		}

		protected virtual Task AfterItemLoadedAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual async Task AfterAllItemsLoadedAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			IEnumerable<Task> tasks = entities.Select(x => AfterItemLoadedAsync(x, cancellationToken, repoOptions, childOptions));

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
		#endregion
	}
}