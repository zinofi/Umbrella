using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Encapsulates read-only access to a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public interface IReadOnlyGenericDbRepository<TEntity> : IReadOnlyGenericDbRepository<TEntity, RepoOptions>
		where TEntity : class, IEntity<int>
	{
	}

	/// <summary>
	/// Encapsulates read-only access to a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	public interface IReadOnlyGenericDbRepository<TEntity, in TRepoOptions> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TRepoOptions : RepoOptions, new()
	{
	}

	/// <summary>
	/// Encapsulates read-only access to a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	public interface IReadOnlyGenericDbRepository<TEntity, in TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		/// <summary>
		/// Finds all entities using the specified parameters.
		/// </summary>
		/// <param name="pageNumber">The page number. Defaults to zero. Pagination will only be applied when this is greater than zero.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="trackChanges">if set to <c>true</c>, tracking entries are created in the database context (where supported).</param>
		/// <param name="map">The include map to specify related entities to load at the same time.</param>
		/// <param name="sortExpressions">The sort expressions.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="filterExpressionCombinator">The filter expression combinator.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child repo options.</param>
		/// <param name="coreFilterExpression">An additional filter expression to be applied to the query before the <paramref name="filterExpressions"/> and any additional <paramref name="additionalFilterExpressions"/> are applied.</param>
		/// <param name="additionalFilterExpressions">Optional additional filter expressions that are too complex to model using the <see cref="FilterExpression{TItem}"/> type.</param>
		/// <returns>The paginated results.</returns>
		Task<PaginatedResultModel<TEntity>> FindAllAsync(
			int pageNumber = 0,
			int pageSize = 20,
			CancellationToken cancellationToken = default,
			bool trackChanges = false,
			IncludeMap<TEntity>? map = null,
			IEnumerable<SortExpression<TEntity>>? sortExpressions = null,
			IEnumerable<FilterExpression<TEntity>>? filterExpressions = null,
			FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or,
			TRepoOptions? repoOptions = null,
			IEnumerable<RepoOptions>? childOptions = null,
			Expression<Func<TEntity, bool>>? coreFilterExpression = null,
			IEnumerable<Expression<Func<TEntity, bool>>>? additionalFilterExpressions = null);

		/// <summary>
		/// Finds an entity in the database using its id.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="trackChanges">if set to <c>true</c>, tracking entries are created in the database context (where supported).</param>
		/// <param name="map">The include map to specify related entities to load at the same time</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child repo options.</param>
		/// <returns>The entity.</returns>
		Task<TEntity?> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity>? map = null, TRepoOptions? repoOptions = null, IEnumerable<RepoOptions>? childOptions = null);

		/// <summary>
		/// Finds all entities specified in the list of <paramref name="entityKeys"/>.
		/// </summary>
		/// <param name="entityKeys">The entity keys.</param>
		/// <param name="trackChanges">if set to <c>true</c>, tracking entries are created in the database context (where supported).</param>
		/// <param name="map">The include map to specify related entities to load at the same time</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child repo options.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The entities.</returns>
		Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> entityKeys, bool trackChanges = false, IncludeMap<TEntity>? map = null, TRepoOptions? repoOptions = null, IEnumerable<RepoOptions>? childOptions = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Finds the total count of all <typeparamref name="TEntity"/> entities in the database.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The total count.</returns>
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Determines if an entity exists in the database using its id.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><see langword="true"/> if it exists; otherwise <see langword="false"/>.</returns>
		Task<bool> ExistsByIdAsync(TEntityKey id, CancellationToken cancellationToken = default);
	}
}