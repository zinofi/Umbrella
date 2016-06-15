using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey> : IGenericRepository<TEntity, TEntityKey, NoopSyncOptions>
        where TEntity : class, IEntity<TEntityKey>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey, TSyncOptions>
        where TEntity : class, IEntity<TEntityKey>
        where TSyncOptions : class, new()
    {
        void RemoveEmptyEntities(ICollection<TEntity> entities);
        void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, bool validateEntity = true, TSyncOptions syncOptions = null);
        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool addToContext = true, bool validateEntity = true, TSyncOptions syncOptions = null);
        void SaveAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false, bool validateEntities = true, TSyncOptions syncOptions = null);
        Task SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool bypassSaveLogic = false, bool validateEntities = true, TSyncOptions syncOptions = null);
        void Delete(TEntity entity, bool pushChangesToDb = true, TSyncOptions syncOptions = null);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TSyncOptions syncOptions = null);
        void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, TSyncOptions syncOptions = null);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TSyncOptions syncOptions = null);
        List<TEntity> FindAll(bool asNoTracking = true, IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool asNoTracking = true, IncludeMap<TEntity> map = null);
        List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool asNoTracking = true, IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool asNoTracking = true, IncludeMap<TEntity> map = null);
        TEntity FindById(TEntityKey id, bool asNoTracking = true, IncludeMap<TEntity> map = null);
        Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool asNoTracking = true, IncludeMap<TEntity> map = null);
        int FindTotalCount();
        Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}