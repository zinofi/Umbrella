using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey> : IGenericRepository<TEntity, TEntityKey, RepoOptions>
        where TEntity : class, IEntity<TEntityKey>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey, in TRepoOptions>
        where TEntity : class, IEntity<TEntityKey>
        where TRepoOptions : class, new()
    {
        void RemoveEmptyEntities(ICollection<TEntity> entities);
        void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void SaveAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void Delete(TEntity entity, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TRepoOptions repoOptions = null, params RepoOptions[] childOptions);
        List<TEntity> FindAll(bool trackChanges = false, IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null);
        List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null);
        TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null);
        Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null);
        int FindTotalCount();
        Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}