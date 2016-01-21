using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        void SetIncludeMap(IIncludeMap<TEntity> map);
        bool NoTracking { get; set; }
        bool IsEmptyEntity(TEntity entity);
        void SanitizeEntity(TEntity entity);
        void RemoveEmptyEntities(ICollection<TEntity> entities);
        void Save(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true, bool addToContext = true);
        Task SaveAsync(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true, bool addToContext = true);
        void SaveAll(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool bypassSaveLogic = false, bool pushChangesToDb = true);
        Task SaveAllAsync(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool bypassSaveLogic = false, bool pushChangesToDb = true);
        void Delete(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true);
        Task DeleteAsync(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true);
        void DeleteAll(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool pushChangesToDb = true);
        Task DeleteAllAsync(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool pushChangesToDb = true);
        List<TEntity> FindAll();
        Task<List<TEntity>> FindAllAsync();
        List<TEntity> FindAllByIdList(IEnumerable<TKey> ids);
        Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TKey> ids);
        TEntity FindById(TKey id);
        Task<TEntity> FindByIdAsync(TKey id);
        int FindTotalCount();
        Task<int> FindTotalCountAsync();
    }
}
