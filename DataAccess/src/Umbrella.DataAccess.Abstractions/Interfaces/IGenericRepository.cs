using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Abstractions.Interfaces
{
	// TODO: V3 Rename these to be more reflective of their use with databases, e.g. EF, or Maybe put Db in the names somewhere?
    public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey> : IGenericRepository<TEntity, TEntityKey, RepoOptions>
        where TEntity : class, IEntity<TEntityKey>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey, in TRepoOptions> : IReadOnlyGenericRepository<TEntity, TEntityKey>
        where TEntity : class, IEntity<TEntityKey>
        where TRepoOptions : class, new()
    {
        void RemoveEmptyEntities(ICollection<TEntity> entities, RepoOptions[] options);
        Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken, RepoOptions[] options);
        void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void SaveAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void Delete(TEntity entity, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
    }
}