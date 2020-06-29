using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.EntityFrameworkCore.Extensions;
using Umbrella.Utilities;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Serves as the base class for repositories which provide read-only access to entities stored in a database accessed using Entity Framework Core.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <seealso cref="T:Umbrella.DataAccess.EntityFrameworkCore.ReadOnlyGenericDbRepository{TEntity, TDbContext, Umbrella.DataAccess.Abstractions.RepoOptions, System.Int32}" />
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext> : ReadOnlyGenericDbRepository<TEntity, TDbContext, RepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : DbContext
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
	/// <seealso cref="T:Umbrella.DataAccess.EntityFrameworkCore.ReadOnlyGenericDbRepository{TEntity, TDbContext, Umbrella.DataAccess.Abstractions.RepoOptions, System.Int32}" />
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : DbContext
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
	/// <seealso cref="T:Umbrella.DataAccess.EntityFrameworkCore.ReadOnlyGenericDbRepository{TEntity, TDbContext, Umbrella.DataAccess.Abstractions.RepoOptions, System.Int32}" />
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : DbContext
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
	/// <seealso cref="T:Umbrella.DataAccess.EntityFrameworkCore.ReadOnlyGenericDbRepository{TEntity, TDbContext, Umbrella.DataAccess.Abstractions.RepoOptions, System.Int32}" />
	public abstract class ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : DbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
		where TUserAuditKey : IEquatable<TUserAuditKey>
	{
		#region Protected Static Properties
		/// <summary>
		/// Gets the default repo options.
		/// </summary>
		protected static TRepoOptions DefaultRepoOptions = new TRepoOptions();

		/// <summary>
		/// Gets or sets the valid filter expressions that can be applied to queries. This should be initialized inside the static constructor
		/// of the derived class.
		/// </summary>
		protected static IReadOnlyCollection<Expression<Func<TEntity, object>>>? ValidFilters { get; set; }
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
		protected IQueryable<TEntity> Items => ApplyQueryFilter(Context.Set<TEntity>());

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
		public virtual async Task<(IReadOnlyCollection<TEntity> results, int totalCount)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null!, IEnumerable<SortExpression<TEntity>> sortExpressions = null!, IEnumerable<FilterExpression<TEntity>> filterExpressions = null!, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or, TRepoOptions? repoOptions = null, IEnumerable<RepoOptions>? childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				ValidateFilters(filterExpressions);

				var filteredQuery = Items.ApplyFilterExpressions(filterExpressions, filterExpressionCombinator);

				int totalCount = await filteredQuery.CountAsync();
				List<TEntity> entities = await filteredQuery
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
		public virtual async Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity>? map = null, TRepoOptions? repoOptions = null, IEnumerable<RepoOptions>? childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var entity = await Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken).ConfigureAwait(false);

				if (entity == null)
					return null!;

				await ThrowIfCannotAcesssAsync(entity, cancellationToken).ConfigureAwait(false);

				await AfterItemLoadedAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				return entity;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, trackChanges, map, repoOptions, childOptions }, returnValue: true))
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
		/// <summary>
		/// Validates that the specified filters are permitted by the <see cref="ValidFilters"/>.
		/// </summary>
		/// <param name="filters">The filters.</param>
		/// <returns><see langword="true"/> if they are all contained in the <see cref="ValidFilters"/>; otherwise <see langword="false"/>.</returns>
		protected virtual bool ValidateFilters(IEnumerable<FilterExpression<TEntity>> filters)
		{
			if (filters is null || ValidFilters is null)
				return true;

			// TODO - UM: We need to create an Expression cache.
			// Create it as a Utility or Helper (or something like that). Just get the type names of the parameters (fullname)
			// and then get the membername of each expression to form the key. If we use the cache in the model binders properly
			// we will be sharing instances meaning we can massively cut down on memory usage and CPU usage.
			// We could also do something similar for the Func properties on the Expressions.
			// Call it a DataExpressionCache! Not sure how to resolve Func though on the Filter and Sort instances, hmmm...
			// return filters.All(x => ValidFilters.Contains(x.Expression));

			return filters.All(x => ValidFilters.Select(x => x.ToString()).Contains(x.Expression.ToString()));
		}

		/// <summary>
		/// Applies the query filter. By default, this does nothing unless overridden by derived types.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>An updated query with the filter applied.</returns>
		protected virtual IQueryable<TEntity> ApplyQueryFilter(IQueryable<TEntity> query) => query;

		/// <summary>
		/// Determines whether this entity can be accessed in the current context. By default this returns <see langword="true"/>
		/// unless overridden by derived types.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><see langword="true"/> if it can be accessed; otherwise <see langword="false"/>.</returns>
		protected virtual Task<bool> CanAccessAsync(TEntity entity, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}

		/// <summary>
		/// Throws an exception if the specified entity cannot be accessed in the current context.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="UmbrellaDataAccessForbiddenException"></exception>
		protected async Task ThrowIfCannotAcesssAsync(TEntity entity, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!await CanAccessAsync(entity, cancellationToken).ConfigureAwait(false))
				throw new UmbrellaDataAccessForbiddenException("The specified entity cannot be accessed in the current context.");
		}


		protected async Task FilterByAccessAsync(List<TEntity> entities, bool throwAccessException, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			for (int i = 0; i < entities.Count; i++)
			{
				var entity = entities[i];

				if (!await CanAccessAsync(entity, cancellationToken).ConfigureAwait(false))
				{
					Log.WriteWarning(state: new { Type = entity.GetType().FullName, entity.Id }, message: "The specified item failed the access check and has been filterd out. This should not happen and means that the query filter is not sufficient.");

					if (throwAccessException)
						throw new UmbrellaDataAccessForbiddenException();

					entities.RemoveAt(i);
					i--;
				}
			}
		}

		protected virtual Task AfterItemLoadedAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions? repoOptions, IEnumerable<RepoOptions>? childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected async Task AfterAllItemsLoadedAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, TRepoOptions? repoOptions, IEnumerable<RepoOptions>? childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			IEnumerable<Task> tasks = entities.Select(x => AfterItemLoadedAsync(x, cancellationToken, repoOptions, childOptions));

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
		#endregion
	}
}