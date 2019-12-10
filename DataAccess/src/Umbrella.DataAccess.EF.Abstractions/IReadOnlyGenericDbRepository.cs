using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.EF.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IReadOnlyGenericDbRepository<TEntity> : IReadOnlyGenericDbRepository<TEntity, RepoOptions>
		where TEntity : class, IEntity<int>
	{
	}

	public interface IReadOnlyGenericDbRepository<TEntity, in TRepoOptions> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TRepoOptions : RepoOptions, new()
	{
	}

	public interface IReadOnlyGenericDbRepository<TEntity, in TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TRepoOptions : RepoOptions, new()
	{
		Task<(IReadOnlyCollection<TEntity> results, int totalCount)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task<(IReadOnlyCollection<TEntity> results, int totalCount)> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, IEnumerable<SortExpression<TEntity>> sortExpressions = null, IEnumerable<FilterExpression<TEntity>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}