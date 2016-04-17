using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IGenericRepository<TEntity, TEntityKey> where TEntity : class, IEntity<TEntityKey>
    {
        bool IsEmptyEntity(TEntity entity);
        void SanitizeEntity(TEntity entity);
        void RemoveEmptyEntities(ICollection<TEntity> entities);
        void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true);
        Task SaveAsync(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, CancellationToken cancellationToken = default(CancellationToken));
        void SaveAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false);
        Task SaveAllAsync(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false, CancellationToken cancellationToken = default(CancellationToken));
        void Delete(TEntity entity, bool pushChangesToDb = true);
        Task DeleteAsync(TEntity entity, bool pushChangesToDb = true, CancellationToken cancellationToken = default(CancellationToken));
        void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, bool pushChangesToDb = true, CancellationToken cancellationToken = default(CancellationToken));
        List<TEntity> FindAll(IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllAsync(IncludeMap<TEntity> map = null, CancellationToken cancellationToken = default(CancellationToken));
        List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, IncludeMap<TEntity> map = null);
        Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, IncludeMap<TEntity> map = null, CancellationToken cancellationToken = default(CancellationToken));
        TEntity FindById(TEntityKey id, IncludeMap<TEntity> map = null);
        Task<TEntity> FindByIdAsync(TEntityKey id, IncludeMap<TEntity> map = null, CancellationToken cancellationToken = default(CancellationToken));
        int FindTotalCount();
        Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}