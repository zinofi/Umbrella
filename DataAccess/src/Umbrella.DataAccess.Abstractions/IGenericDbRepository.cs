using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IGenericDbRepository<TEntity> : IGenericDbRepository<TEntity, RepoOptions>
		where TEntity : class, IEntity<int>
	{
	}

	public interface IGenericDbRepository<TEntity, in TRepoOptions> : IGenericDbRepository<TEntity, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TRepoOptions : RepoOptions, new()
	{
	}

	public interface IGenericDbRepository<TEntity, in TRepoOptions, TEntityKey> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TRepoOptions : RepoOptions, new()
	{
		Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task<SaveResult<TEntity>> SaveAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task<IReadOnlyCollection<SaveResult<TEntity>>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
		Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
	}
}