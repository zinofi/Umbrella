using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace Umbrella.DataAccess
{
    public abstract class GenericCachedRepository<TEntity, TDbContext> : GenericCachedRepository<TEntity, TDbContext, int, int, IEntity<int>>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext, new()
    {
        public GenericCachedRepository(IDataContextFactory<TDbContext> dataContextFactory, IUserAuditDataFactory<int> userAuditDataFactory, ILogger logger)
            : base(dataContextFactory, userAuditDataFactory, logger)
        {
        }
    }

    public abstract class GenericCachedRepository<TEntity, TDbContext, TUserAuditKey> : GenericCachedRepository<TEntity, TDbContext, int, TUserAuditKey, IEntity<int>>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext, new()
    {
        public GenericCachedRepository(IDataContextFactory<TDbContext> dataContextFactory, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
            : base(dataContextFactory, userAuditDataFactory, logger)
        {
        }
    }

    public abstract class GenericCachedRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey> : GenericCachedRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey, IEntity<TEntityKey>>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext, new()
    {
        public GenericCachedRepository(IDataContextFactory<TDbContext> dataContextFactory, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
            : base(dataContextFactory, userAuditDataFactory, logger)
        {
        }
    }

    public abstract class GenericCachedRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey, TEntityBase> : GenericRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey, TEntityBase>
        where TEntity : class, TEntityBase
        where TDbContext : DbContext, new()
        where TEntityBase : IEntity<TEntityKey>
    {
        protected static readonly object s_Lock = new object();

        protected static List<TEntity> s_EntityList = new List<TEntity>();
        protected static Dictionary<int, TEntity> s_EntityDictionary = new Dictionary<int, TEntity>();

        public GenericCachedRepository(IDataContextFactory<TDbContext> dataContextFactory, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
            : base(dataContextFactory, userAuditDataFactory, logger)
        {
            //AfterContextSavedChanges += (repo, entity) => AddOrUpdateCache(new[] { entity });
            //AfterContextSavedChangesMultiple += (repo, entities) => AddOrUpdateCache(entities.ToArray());

            //AfterContextDeletedChanges += (repo, entity) => RemoveFromCache(new[] { entity });
            //AfterContextDeletedChangesMultiple += (repo, entities) => RemoveFromCache(entities.ToArray());
        }

        //public override TEntity FindById(int id)
        //{
        //    if (!s_EntityDictionary.ContainsKey(id))
        //    {
        //        lock(s_Lock)
        //        {
        //            if(!s_EntityDictionary.ContainsKey(id))
        //            {
        //                TEntity entity = base.FindById(id);
                        
        //                if (entity != null)
        //                    s_EntityDictionary.Add(id, entity);
        //                else
        //                    return null;
        //            }
        //        }
        //    }

        //    return s_EntityDictionary[id];
        //}

        //public override List<TEntity> FindAll()
        //{
        //    int totalCount = Items.Count();

        //    if (s_EntityList.Count != totalCount)
        //    {
        //        lock (s_Lock)
        //        {
        //            if (s_EntityList.Count != totalCount)
        //            {
        //                //Count is not the same which means that the cache has already been partially filled
        //                //Just replace it in its entirety with the contents of the database
        //                s_EntityList = Items.ToList();
        //                s_EntityDictionary = s_EntityList.ToDictionary(x => x.Id);
        //            }
        //        }
        //    }

        //    //Go to the database and get the count of all items
        //    return s_EntityList;
        //}

        //protected virtual void AddOrUpdateCache(IEnumerable<TEntity> entities)
        //{
        //    lock (s_Lock)
        //    {
        //        foreach (TEntity entity in entities)
        //        {
        //            TEntity cached = s_EntityDictionary.ContainsKey(entity.Id) ? s_EntityDictionary[entity.Id] : null;
        //            if (cached != null)
        //            {
        //                //Remove from both the dictionary and list
        //                s_EntityList.Remove(cached);
        //                s_EntityDictionary.Remove(cached.Id);
        //            }

        //            //Add this into dictionary and list
        //            s_EntityList.Add(entity);
        //            s_EntityDictionary.Add(entity.Id, entity);
        //        }
        //    }
        //}

        //protected virtual void RemoveFromCache(IEnumerable<TEntity> entities)
        //{
        //    lock(s_Lock)
        //    {
        //        foreach(TEntity entity in entities)
        //        {
        //            s_EntityList.Remove(entity);
        //            s_EntityDictionary.Remove(entity.Id);
        //        }
        //    }
        //}

        //protected virtual void ClearCache()
        //{
        //    lock(s_Lock)
        //    {
        //        s_EntityList = new List<TEntity>();
        //        s_EntityDictionary = new Dictionary<int, TEntity>();
        //    }
        //}
    }
}