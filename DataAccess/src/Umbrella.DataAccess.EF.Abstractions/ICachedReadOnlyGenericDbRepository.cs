using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EF.Abstractions
{
	// TODO: V3 - Alter the FindAllMethods to accept pagination parameters.
	public interface ICachedReadOnlyGenericDbRepository<TEntity> : ICachedReadOnlyGenericDbRepository<TEntity, int>
		where TEntity : class, IEntity<int>
	{
	}

	public interface ICachedReadOnlyGenericDbRepository<TEntity, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
	{
		Task<IReadOnlyCollection<CacheEntry<TEntity>>> CacheFindAllAsync(CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
		Task<CacheEntry<TEntity>> CacheFindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default, bool trackChanges = false, IncludeMap<TEntity> map = null);
	}
}