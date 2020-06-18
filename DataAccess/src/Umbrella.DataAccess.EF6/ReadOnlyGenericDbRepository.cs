using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.EF6.Extensions;
using Umbrella.Utilities;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.EF6
{
	/// <summary>
	/// Serves as the base class for repositories which provide read-only access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext> : ReadOnlyGenericDbRepository<TEntity, TDbContext, RepoOptions>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyGenericDbRepository{TEntity, TDbContext}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
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

	/// <summary>
	/// Serves as the base class for repositories which provide read-only access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
	{
		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyGenericDbRepository{TEntity, TDbContext, TRepoOptions}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
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

	/// <summary>
	/// Serves as the base class for repositories which provide read-only access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyGenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
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

	/// <summary>
	/// Serves as the base class for repositories which provide read-only access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TUserAuditKey">The type of the user audit key.</typeparam>
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
		where TUserAuditKey : IEquatable<TUserAuditKey>
	{
		#region Protected Static Properties		
		/// <summary>
		/// Gets the default repo options.
		/// </summary>
		protected static TRepoOptions DefaultRepoOptions { get; } = new TRepoOptions();
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the database context.
		/// </summary>
		protected TDbContext Context { get; }

		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the lookup normalizer.
		/// </summary>
		protected ILookupNormalizer LookupNormalizer { get; }

		/// <summary>
		/// Gets the <see cref="IQueryable{TEntity}"/> from the database context for the current <typeparamref name="TEntity"/> type.
		/// </summary>
		protected IQueryable<TEntity> Items => Context.Set<TEntity>();

		/// <summary>
		/// Gets the current user identifier.
		/// </summary>
		protected TUserAuditKey CurrentUserId => CurrentUserIdAccessor.CurrentUserId;

		/// <summary>
		/// Gets the current user identifier accessor.
		/// </summary>
		protected ICurrentUserIdAccessor<TUserAuditKey> CurrentUserIdAccessor { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyGenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
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
		/// <inheritdoc />
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

				await FilterByAccessAsync(entities, false, cancellationToken).ConfigureAwait(false);
				await AfterAllItemsLoadedAsync(entities, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return (entities, totalCount);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, trackChanges, map, sortExpressions = sortExpressions.ToSortExpressionSerializables(), filterExpressions = filterExpressions.ToFilterExpressionSerializables(), filterExpressionCombinator, repoOptions, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items using the specified parameters.", exc);
			}
		}

		/// <inheritdoc />
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

				await FilterByAccessAsync(entities, false, cancellationToken).ConfigureAwait(false);
				await AfterAllItemsLoadedAsync(entities, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return (entities, totalCount);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ids, trackChanges, map, sortExpressions = sortExpressions.ToSortExpressionSerializables(), filterExpressions = filterExpressions.ToFilterExpressionSerializables(), filterExpressionCombinator, repoOptions, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving all items with the specified ids.", exc);
			}
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
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

		protected async Task FilterByAccessAsync(List<TEntity> entities, bool throwAccessException, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			for (int i = 0; i < entities.Count; i++)
			{
				var entity = entities[i];

				if (!await CanAccessAsync(entity, cancellationToken).ConfigureAwait(false))
				{
					Log.WriteWarning(state: new { Type = entity.GetType().FullName, entity.Id }, message: "The specified item failed the access check. This should not happen.");

					if (throwAccessException)
						throw new UmbrellaDataAccessForbiddenException();

					entities.RemoveAt(i);
					i--;
				}
			}
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