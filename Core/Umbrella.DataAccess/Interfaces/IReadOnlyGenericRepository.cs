using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IReadOnlyGenericRepository<TEntity> : IReadOnlyGenericRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IReadOnlyGenericRepository<TEntity, TEntityKey>
        where TEntity : class, IEntity<TEntityKey>
    {
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