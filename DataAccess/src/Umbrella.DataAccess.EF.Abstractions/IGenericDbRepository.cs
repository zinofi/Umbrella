using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EF.Abstractions
{
	public interface IGenericDbRepository<TEntity> : IGenericDbRepository<TEntity, int>
		where TEntity : class, IEntity<int>
	{
	}

	public interface IGenericDbRepository<TEntity, TEntityKey> : IGenericDbRepository<TEntity, TEntityKey, RepoOptions>
		where TEntity : class, IEntity<TEntityKey>
	{
	}

	public interface IGenericDbRepository<TEntity, TEntityKey, in TRepoOptions> : IReadOnlyGenericDbRepository<TEntity, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TRepoOptions : class, new()
	{
		Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken, RepoOptions[] options);
		Task<SaveResult<TEntity>> SaveAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
		Task<IReadOnlyCollection<SaveResult<TEntity>>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
		Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
		Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
	}
}