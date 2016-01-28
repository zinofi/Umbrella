using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        void SetIncludeMap(IIncludeMap<TEntity> map);
        bool NoTracking { get; set; }
        bool IsEmptyEntity(TEntity entity);
        void SanitizeEntity(TEntity entity);
        void RemoveEmptyEntities(ICollection<TEntity> entities);
        void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true);
        Task SaveAsync(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, CancellationToken cancellationToken = default(CancellationToken));
        void SaveAll(IEnumerable<TEntity> entities, bool bypassSaveLogic = false, bool pushChangesToDb = true);
        Task SaveAllAsync(IEnumerable<TEntity> entities, bool bypassSaveLogic = false, bool pushChangesToDb = true, CancellationToken cancellationToken = default(CancellationToken));
        void Delete(TEntity entity, bool pushChangesToDb = true);
        Task DeleteAsync(TEntity entity, bool pushChangesToDb = true, CancellationToken cancellationToken = default(CancellationToken));
        void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, bool pushChangesToDb = true, CancellationToken cancellationToken = default(CancellationToken));
        List<TEntity> FindAll();
        Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken));
        List<TEntity> FindAllByIdList(IEnumerable<TKey> ids);
        Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default(CancellationToken));
        TEntity FindById(TKey id);
        Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken));
        int FindTotalCount();
        Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
