using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.EF.Abstractions;
using Umbrella.Utilities.Sorting;

namespace Umbrella.DataAccess.Abstractions
{
	// TODO: V3 - Alter the FindAllMethods to accept pagination parameters.
	public interface IReadOnlyGenericDbRepository<TEntity> : IReadOnlyGenericDbRepository<TEntity, int>
		where TEntity : class, IEntity<int>
	{
	}

	public interface IReadOnlyGenericDbRepository<TEntity, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
	{
		Task<IReadOnlyCollection<TEntity>> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, params SortExpression<TEntity>[] sortExpressions);
		Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null, params SortExpression<TEntity>[] sortExpressions);
		Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}