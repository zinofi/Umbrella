using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Data.Entity;
using System.Linq.Expressions;
using Umbrella.DataAccess.Interfaces;
using Microsoft.Data.Entity.Infrastructure;
using Umbrella.Utilities;
using System.Threading.Tasks;
using Umbrella.Utilities.Enumerations;
using Umbrella.Utilities.Extensions;
using Umbrella.DataAccess.Exceptions;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Umbrella.DataAccess
{
    public abstract class GenericRepository<TEntity, TDbContext> : GenericRepository<TEntity, TDbContext, int, int, IEntity<int>>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext, new()
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<int> userAuditDataFactory, ILogger logger)
            : base(dbContext, userAuditDataFactory, logger)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TDbContext, TUserAuditKey> : GenericRepository<TEntity, TDbContext, int, TUserAuditKey, IEntity<int>>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext, new()
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
            : base(dbContext, userAuditDataFactory, logger)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey> : GenericRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey, IEntity<TEntityKey>>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext, new()
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
            : base(dbContext, userAuditDataFactory, logger)
        {
        }
    }

    /// <summary>
    /// A general purpose base class containing core repository functionality.
    /// </summary>
    /// <typeparam name="TEntity">The type of the generated entity, e.g. Person, Car</typeparam>
    /// <typeparam name="TDbContext">The type of the data context</typeparam>
    /// <typeparam name="TEntityBase">The entity interface implemented by the generated entities. Must derive from IEntity</typeparam>
    public abstract class GenericRepository<TEntity, TDbContext, TEntityKey, TUserAuditKey, TEntityBase> : IGenericRepository<TEntity, TEntityKey>
        where TEntity : class, TEntityBase
        where TDbContext : DbContext, new()
        where TEntityBase : IEntity<TEntityKey>
    {
        #region Private Constants
        private const string c_InvalidPropertyStringLengthErrorMessageFormat = "The {0} value must be between {1} and {2} characters in length.";
        private const string c_InvalidPropertyNumberRangeErrorMessageFormat = "The {0} value must be between {1} and {2}.";
        #endregion

        #region Private Static Members
        private static readonly object s_Lock = new object();
        #endregion

        #region Private Members
        private TDbContext m_DbContext;
        private IUserAuditDataFactory<TUserAuditKey> m_UserAuditDataFactory;
        private IIncludeMap<TEntity> m_IncludeMap;
        #endregion

        #region Protected Members
        protected readonly ILogger m_Loggger;
        #endregion

        #region Protected Properties
        protected TDbContext Context => m_DbContext;

        protected TUserAuditKey CurrentUserId => m_UserAuditDataFactory.CurrentUserId;

        protected IQueryable<TEntity> Items
        {
            get
            {
                IQueryable<TEntity> items = Context.Set<TEntity>();

                if (m_IncludeMap != null)
                {
                    foreach (var path in m_IncludeMap.Includes)
                    {
                        items = items.Include(path);
                    }
                }

                if (NoTracking)
                    items = items.AsNoTracking();

                return items;
            }
        }
        #endregion

        #region Public Properties
        public bool NoTracking { get; set; }
        #endregion

        #region Constructors
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger)
        {
            m_DbContext = dbContext;
            m_UserAuditDataFactory = userAuditDataFactory;
            m_Loggger = logger;
        }
        #endregion

        #region Save
        public void Save(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true, bool addToContext = true)
        {
            try
            {
                //Additional processing before changes have been reflected in the database context
                BeforeContextSaving(entity);

                ValidateEntity(entity);

                //Common work shared between the synchronous and asynchronous version of the Save method
                PreSaveWork(entity, enableEntityValidation, addToContext);

                //Additional processing after changes have been reflected in the database context but not yet pushed to the database
                AfterContextSaving(entity);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    Context.SaveChanges();

                    //Additional processing after changes have been successfully committed to the database
                    AfterContextSavedChanges(entity);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation, addToContext }, "Concurrency Exception for Id"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation, addToContext }, "Failed for Id"))
            {
                throw;
            }
        }

        public async Task SaveAsync(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true, bool addToContext = true)
        {
            try
            {
                //Additional processing before changes have been reflected in the database context
                await BeforeContextSavingAsync(entity);

                //Validate Entity
                await ValidateEntityAsync(entity);

                //Common work shared between the synchronous and asynchronous version of the Save method
                PreSaveWork(entity, enableEntityValidation, addToContext);

                //Additional processing after changes have been reflected in the database context but not yet pushed to the database
                await AfterContextSavingAsync(entity);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    await Context.SaveChangesAsync();

                    //Additional processing after changes have been successfully committed to the database
                    await AfterContextSavedChangesAsync(entity);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation, addToContext }, "Concurrency Exception for Id"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation, addToContext }, "Failed for Id"))
            {
                throw;
            }
        }

        private void PreSaveWork(TEntity entity, bool enableEntityValidation, bool addToContext)
        {
            //Look for the entity in the context - this action will allow us to determine it's state
            EntityEntry<TEntity> dbEntity = Context.Entry<TEntity>(entity);

            IDateAuditEntity datedEntity = entity as IDateAuditEntity;
            IUserAuditEntity<TUserAuditKey> userAuditEntity = entity as IUserAuditEntity<TUserAuditKey>;

            //Check if this entity is in the context, i.e. is it new
            if (entity.Id.Equals(default(TEntityKey)) && (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached)))
            {
                if (datedEntity != null)
                    datedEntity.CreatedDate = DateTime.Now;

                if (userAuditEntity != null)
                    userAuditEntity.CreatedById = m_UserAuditDataFactory.CurrentUserId;

                if (addToContext)
                    Context.Set<TEntity>().Add(entity);
            }

            if (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached) || dbEntity.State.HasFlag(EntityState.Modified))
            {
                if (datedEntity != null)
                    datedEntity.UpdatedDate = DateTime.Now;

                if (userAuditEntity != null)
                    userAuditEntity.UpdatedById = m_UserAuditDataFactory.CurrentUserId;
            }

            //TODO: Context.Configuration.ValidateOnSaveEnabled = enableEntityValidation;
        }

        /// <summary>
        /// Save All entities in a single transaction
        /// </summary>
        /// <param name="entities">The entities to be saved in a single transaction</param>
        /// <param name="enableEntityValidation">Determines if entity validation should be performed</param>
        /// <param name="bypassSaveLogic">This should almost always be set to true - you should never have to bypass the default logic except in exceptional cases! When bypassing, you'll have to do the work yourself!</param>
        public void SaveAll(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool bypassSaveLogic = false, bool pushChangesToDb = true)
        {
            try
            {
                //Save all changes - do not push to the database yet
                if (!bypassSaveLogic)
                {
                    foreach (TEntity entity in entities)
                    {
                        Save(entity, false, enableEntityValidation);
                    }
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    Context.SaveChanges();

                    //Additional process after all changes have been committed to the database
                    AfterContextSavedChangesMultiple(entities);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { enableEntityValidation, bypassSaveLogic }, "Concurrency Exception"))
            {   
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { enableEntityValidation, bypassSaveLogic }))
            {
                throw;
            }
        }

        public async Task SaveAllAsync(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool bypassSaveLogic = false, bool pushChangesToDb = true)
        {
            try
            {
                //Save all changes - do not push to the database yet
                if (!bypassSaveLogic)
                {
                    foreach (TEntity entity in entities)
                    {
                        await SaveAsync(entity, false, enableEntityValidation);
                    }
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    await Context.SaveChangesAsync();

                    //Additional process after all changes have been committed to the database
                    await AfterContextSavedChangesMultipleAsync(entities);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { enableEntityValidation, bypassSaveLogic }, "Concurrency Exception"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { enableEntityValidation, bypassSaveLogic }))
            {
                throw;
            }
        }
        #endregion

        #region Delete
        public void Delete(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true)
        {
            try
            {
                BeforeContextDeleting(entity);

                PreDeleteWork(entity, enableEntityValidation);

                AfterContextDeleting(entity);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    Context.SaveChanges();

                    AfterContextDeletedChanges(entity);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation }, "Concurrency Exception for Id"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation }, "Failed for Id"))
            {
                throw;
            }
        }

        public async Task DeleteAsync(TEntity entity, bool pushChangesToDb = true, bool enableEntityValidation = true)
        {
            try
            {
                await BeforeContextDeletingAsync(entity);

                PreDeleteWork(entity, enableEntityValidation);

                await AfterContextDeletingAsync(entity);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    await Context.SaveChangesAsync();

                    await AfterContextDeletedChangesAsync(entity);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation }, "Concurrency Exception for Id"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { entity.Id, pushChangesToDb, enableEntityValidation }, "Failed for Id"))
            {
                throw;
            }
        }

        private void PreDeleteWork(TEntity entity, bool enableEntityValidation)
        {
            Context.Set<TEntity>().Remove(entity);
            Context.Entry(entity).State = EntityState.Deleted;

            //TODO: Context.Configuration.ValidateOnSaveEnabled = enableEntityValidation;
        }

        /// <summary>
        /// Delete all entities in a single transaction
        /// </summary>
        /// <param name="entities">The entities to be deleted</param>
        /// <param name="enableEntityValidation">Perform entity validation</param>
        public void DeleteAll(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool pushChangesToDb = true)
        {
            try
            {
                foreach (TEntity entity in entities)
                {
                    Delete(entity, false, enableEntityValidation);
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    Context.SaveChanges();

                    AfterContextDeletedChangesMultiple(entities);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { enableEntityValidation }, "Concurrency Exception"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { enableEntityValidation }))
            {
                throw;
            }
        }

        public async Task DeleteAllAsync(IEnumerable<TEntity> entities, bool enableEntityValidation = true, bool pushChangesToDb = true)
        {
            try
            {
                foreach (TEntity entity in entities)
                {
                    await DeleteAsync(entity, false, enableEntityValidation);
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    await Context.SaveChangesAsync();

                    await AfterContextDeletedChangesMultipleAsync(entities);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (m_Loggger.LogError(exc, new { enableEntityValidation }, "Concurrency Exception"))
            {
                throw;
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, new { enableEntityValidation }))
            {
                throw;
            }
        }
        #endregion

        #region Protected Methods

        #region Events
        protected virtual void ValidateEntity(TEntity entity)
        {
        }

        protected virtual Task ValidateEntityAsync(TEntity entity)
        {
            return Task.Run(() => ValidateEntity(entity));
        }

        protected virtual void BeforeContextSaving(TEntity entity)
        {
        }

        protected virtual Task BeforeContextSavingAsync(TEntity entity)
        {
            return Task.Run(() => BeforeContextSaving(entity));
        }

        protected virtual void AfterContextSaving(TEntity entity)
        {
        }

        protected virtual Task AfterContextSavingAsync(TEntity entity)
        {
            return Task.Run(() => AfterContextSaving(entity));
        }

        protected virtual void AfterContextSavedChanges(TEntity entity)
        {
        }

        protected virtual Task AfterContextSavedChangesAsync(TEntity entity)
        {
            return Task.Run(() => AfterContextSavedChanges(entity));
        }

        protected virtual void AfterContextSavedChangesMultiple(IEnumerable<TEntity> entities)
        {
        }

        protected virtual Task AfterContextSavedChangesMultipleAsync(IEnumerable<TEntity> entities)
        {
            return Task.Run(() => AfterContextSavedChangesMultiple(entities));
        }

        protected virtual void BeforeContextDeleting(TEntity entity)
        {
        }

        protected virtual Task BeforeContextDeletingAsync(TEntity entity)
        {
            return Task.Run(() => BeforeContextDeleting(entity));
        }

        protected virtual void AfterContextDeleting(TEntity entity)
        {
        }

        protected virtual Task AfterContextDeletingAsync(TEntity entity)
        {
            return Task.Run(() => AfterContextDeleting(entity));
        }

        protected virtual void AfterContextDeletedChanges(TEntity entity)
        {
        }

        protected virtual Task AfterContextDeletedChangesAsync(TEntity entity)
        {
            return Task.Run(() => AfterContextDeletedChanges(entity));
        }

        protected virtual void AfterContextDeletedChangesMultiple(IEnumerable<TEntity> entities)
        {
        }

        protected virtual Task AfterContextDeletedChangesMultipleAsync(IEnumerable<TEntity> entities)
        {
            return Task.Run(() => AfterContextDeletedChangesMultiple(entities));
        }
        #endregion

        #region SyncDependencies
        protected void SyncDependencies<TTargetEntity, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func)
            where TTargetEntity : class, TEntityBase
            where TTargetRepository : IGenericRepository<TTargetEntity, TEntityKey>
        {
            //Copy the incoming list here - this is because the code in foreach declaration below finds all the entities matching the where clause
            //but the problem is that when that happens, the alteredColl parameter is a reference to the same underlying collection. This means
            //any items that have been removed from the incoming alteredColl will be added back to it. To get around this, we need to copy all the items from alteredColl
            //to a new List first to stop this from happening.
            alteredColl = new List<TTargetEntity>(alteredColl);

            //Ensure we have deleted the dependencies (children) we no longer need
            foreach (TTargetEntity entity in Context.Set<TTargetEntity>().Where(func))
            {
                if (!alteredColl.Contains(entity))
                {
                    //Delete the dependency, but do not push changes to the database
                    repository.Delete(entity, false, false);
                }
            }

            foreach (TTargetEntity entity in alteredColl)
            {
                //Look for the entity in the context - this action will allow us to determine it's state
                EntityEntry<TTargetEntity> dbEntity = Context.Entry<TTargetEntity>(entity);

                //Determine entities that have been added or modified - in these cases we need to call Save so that any custom
                //repository logic is executed
                if (dbEntity.State.HasFlag(EntityState.Detached)
                    || dbEntity.State.HasFlag(EntityState.Added)
                    || dbEntity.State.HasFlag(EntityState.Modified)
                    || dbEntity.State.HasFlag(EntityState.Unchanged))
                {
                    //Do not add children to the context at this point. This still allows us to perform our save
                    //logic on the entity, but it also means that should something go wrong that means
                    //persisting the parent entity is not valid, we don't end up in a situation where we have
                    //child objects as part of the context that shouldn't be saved.
                    repository.Save(entity, false, false, false);
                }
            }
        }

        protected async Task SyncDependenciesAsync<TTargetEntity, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func)
            where TTargetEntity : class, TEntityBase
            where TTargetRepository : IGenericRepository<TTargetEntity, TEntityKey>
        {
            //Copy the incoming list here - this is because the code in foreach declaration below finds all the entities matching the where clause
            //but the problem is that when that happens, the alteredColl parameter is a reference to the same underlying collection. This means
            //any items that have been removed from the incoming alteredColl will be added back to it. To get around this, we need to copy all the items from alteredColl
            //to a new List first to stop this from happening.
            alteredColl = new List<TTargetEntity>(alteredColl);

            //Ensure we have deleted the dependencies (children) we no longer need
            foreach (TTargetEntity entity in Context.Set<TTargetEntity>().Where(func))
            {
                if (!alteredColl.Contains(entity))
                {
                    //Delete the dependency, but do not push changes to the database
                    await repository.DeleteAsync(entity, false, false);
                }
            }

            foreach (TTargetEntity entity in alteredColl)
            {
                //Look for the entity in the context - this action will allow us to determine it's state
                EntityEntry<TTargetEntity> dbEntity = Context.Entry<TTargetEntity>(entity);

                //Determine entities that have been added or modified - in these cases we need to call Save so that any custom
                //repository logic is executed
                if (dbEntity.State.HasFlag(EntityState.Detached)
                    || dbEntity.State.HasFlag(EntityState.Added)
                    || dbEntity.State.HasFlag(EntityState.Modified)
                    || dbEntity.State.HasFlag(EntityState.Unchanged))
                {
                    //Do not add children to the context at this point. This still allows us to perform our save
                    //logic on the entity, but it also means that should something go wrong that means
                    //persisting the parent entity is not valid, we don't end up in a situation where we have
                    //child objects as part of the context that shouldn't be saved.
                    await repository.SaveAsync(entity, false, false, false);
                }
            }
        }
        #endregion

        #region Validation

        protected void ValidatePropertyStringLength(string value, Expression<Func<TEntity, string>> propertyAccessor, int minLength, int maxLength, bool required = true)
        {
            if (!value.IsValidLength(minLength, maxLength, !required))
                throw new DataValidationException(string.Format(c_InvalidPropertyStringLengthErrorMessageFormat, propertyAccessor.GetMemberName(), minLength, maxLength));
        }

        protected void ValidatePropertyNumberRange<TProperty>(TProperty? value, Expression<Func<TEntity, TProperty?>> propertyAccessor, TProperty min, TProperty max, bool required = true)
            where TProperty : struct, IComparable<TProperty>
        {
            if (!value.IsValidRange(min, max, !required))
                throw new DataValidationException(string.Format(c_InvalidPropertyNumberRangeErrorMessageFormat, propertyAccessor.GetMemberName(), min, max));
        }

        #endregion

        protected IOrderedQueryable<TEntity> ApplySortOrder<TProperty>(IQueryable<TEntity> unsortedList, Expression<Func<TEntity, TProperty>> expression, SortDirection direction)
        {
            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return unsortedList.OrderBy(expression);
                case SortDirection.Descending:
                    return unsortedList.OrderByDescending(expression);
            }
        }
        #endregion

        #region Public Methods
        public void SetIncludeMap(IIncludeMap<TEntity> map)
        {
            m_IncludeMap = map;
        }

        public virtual bool IsEmptyEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual void SanitizeEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEmptyEntities(ICollection<TEntity> entities)
        {
            List<TEntity> lstToRemove = entities.Where(x => IsEmptyEntity(x)).ToList();

            foreach (TEntity entity in lstToRemove)
            {
                entities.Remove(entity);

                //Make sure it is removed from the Context if it has just been added - make it detached
                EntityEntry<TEntity> dbEntityEntry = Context.Entry(entity);

                if (dbEntityEntry.State == EntityState.Added)
                    dbEntityEntry.State = EntityState.Detached;
            }
        }

        public virtual List<TEntity> FindAll()
        {
            try
            {
                return Items.ToList();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllAsync()
        {
            try
            {
                return Items.ToListAsync();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc))
            {
                throw;
            }
        }

        public virtual List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids)
        {
            try
            {
                return Items.Where(x => ids.Contains(x.Id)).ToList();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, ids))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids)
        {
            try
            {
                return Items.Where(x => ids.Contains(x.Id)).ToListAsync();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, ids))
            {
                throw;
            }
        }

        public virtual TEntity FindById(TEntityKey id)
        {
            try
            {
                return Items.SingleOrDefault(x => x.Id.ToString() == id.ToString());
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, id))
            {
                throw;
            }
        }

        public virtual Task<TEntity> FindByIdAsync(TEntityKey id)
        {
            try
            {
                return Items.SingleOrDefaultAsync(x => x.Id.ToString() == id.ToString());
            }
            catch (Exception exc) when (m_Loggger.LogError(exc, id))
            {
                throw;
            }
        }

        public virtual int FindTotalCount()
        {
            try
            {
                return Items.Count();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc))
            {
                throw;
            }
        }

        public virtual Task<int> FindTotalCountAsync()
        {
            try
            {
                return Items.CountAsync();
            }
            catch (Exception exc) when (m_Loggger.LogError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}