using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.EF.Abstractions;

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
		Task<IReadOnlyCollection<TEntity>> FindAllAsync(CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
		Task<IReadOnlyCollection<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
		Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}