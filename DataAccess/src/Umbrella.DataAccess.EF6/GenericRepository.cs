using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbrella.DataAccess.Interfaces;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.DataAccess.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Umbrella.Utilities;

namespace Umbrella.DataAccess.EF6
{
    public abstract class GenericRepository<TEntity, TDbContext> : GenericRepository<TEntity, TDbContext, RepoOptions>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<int> userAuditDataFactory, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TDbContext, TRepoOptions> : GenericRepository<TEntity, TDbContext, TRepoOptions, int>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<int> userAuditDataFactory, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : GenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<int> userAuditDataFactory, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer)
        {
        }
    }

    /// <summary>
    /// A general purpose base class containing core repository functionality.
    /// </summary>
    /// <typeparam name="TEntity">The type of the generated entity, e.g. Person, Car</typeparam>
    /// <typeparam name="TDbContext">The type of the data context</typeparam>
    public abstract class GenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : IGenericRepository<TEntity, TEntityKey, TRepoOptions>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        #region Private Constants
        private const string c_InvalidPropertyStringLengthErrorMessageFormat = "The {0} value must be between {1} and {2} characters in length.";
        private const string c_InvalidPropertyNumberRangeErrorMessageFormat = "The {0} value must be between {1} and {2}.";
        private const string c_BulkActionConcurrencyExceptionErrorMessage = "A concurrency error has occurred whilst trying to update the items.";
        private const string c_ConcurrencyExceptionErrorMessageFormat = "A concurrency error has occurred whilst trying to save the item with id {0} or one of its dependants.";
        #endregion

        #region Private Static Members
        private static readonly TRepoOptions s_DefaultRepoOptions = new TRepoOptions();
        #endregion

        #region Private Members
        private readonly IUserAuditDataFactory<TUserAuditKey> m_UserAuditDataFactory;
        #endregion

        #region Protected Members
        protected readonly TDbContext Context;
        protected readonly ILogger Log;
        protected readonly IDataAccessLookupNormalizer LookupNormalizer;
        #endregion

        #region Protected Properties
        protected TUserAuditKey CurrentUserId => m_UserAuditDataFactory.CurrentUserId;

        protected IQueryable<TEntity> Items => Context.Set<TEntity>();
        #endregion

        #region Constructors
        public GenericRepository(TDbContext dbContext, IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory, ILogger logger, IDataAccessLookupNormalizer lookupNormalizer)
        {
            Context = dbContext;
            m_UserAuditDataFactory = userAuditDataFactory;
            Log = logger;
            LookupNormalizer = lookupNormalizer;
        }
        #endregion

        #region Save
        public virtual void Save(TEntity entity, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                Guard.ArgumentNotNull(entity, nameof(entity));

                //Ensure the default options are used when not explicitly provided.
                options = options ?? s_DefaultRepoOptions;

                if (options.SanitizeEntity)
                    SanitizeEntity(entity);

                //Additional processing before changes have been reflected in the database context
                BeforeContextSaving(entity, options, childOptions);

                if(options.ValidateEntity)
                    ValidateEntity(entity, options, childOptions);

                //Common work shared between the synchronous and asynchronous version of the Save method
                PreSaveWork(entity, addToContext);

                //Additional processing after changes have been reflected in the database context but not yet pushed to the database
                AfterContextSaving(entity, options, childOptions);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    Context.SaveChanges();

                    //Additional processing after changes have been successfully committed to the database
                    AfterContextSavedChanges(entity, options, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Concurrency Exception for Id", true))
            {
                throw new DataAccessConcurrencyException(string.Format(c_ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
            }
            catch (DbEntityValidationException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Data Validation Exception for Id", true))
            {
                LogDbEntityValidationExceptionDetails(exc);
                throw;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Failed for Id"))
            {
                throw;
            }
        }

        public virtual async Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool addToContext = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guard.ArgumentNotNull(entity, nameof(entity));

                //Ensure the default options are used when not explicitly provided.
                options = options ?? s_DefaultRepoOptions;

                if (options.SanitizeEntity)
                    await SanitizeEntityAsync(entity, cancellationToken);

                //Additional processing before changes have been reflected in the database context
                await BeforeContextSavingAsync(entity, cancellationToken, options, childOptions);

                if(options.ValidateEntity)
                    await ValidateEntityAsync(entity, cancellationToken, options, childOptions);

                //Common work shared between the synchronous and asynchronous version of the Save method
                PreSaveWork(entity, addToContext);

                //Additional processing after changes have been reflected in the database context but not yet pushed to the database
                await AfterContextSavingAsync(entity, cancellationToken, options, childOptions);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    await Context.SaveChangesAsync(cancellationToken);

                    //Additional processing after changes have been successfully committed to the database
                    await AfterContextSavedChangesAsync(entity, cancellationToken, options, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Concurrency Exception for Id", true))
            {
                throw new DataAccessConcurrencyException(string.Format(c_ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
            }
            catch (DbEntityValidationException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Data Validation Exception for Id", true))
            {
                LogDbEntityValidationExceptionDetails(exc);
                throw;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, options, childOptions }, "Failed for Id"))
            {
                throw;
            }
        }

        protected virtual void PreSaveWork(TEntity entity, bool addToContext)
        {
            //Look for the entity in the context - this action will allow us to determine it's state
            DbEntityEntry<TEntity> dbEntity = Context.Entry(entity);

            //TODO: Replace with C# 7 Pattern Matching when available
            IConcurrencyStamp concurrencyStampEntity = entity as IConcurrencyStamp;
            IDateAuditEntity datedEntity = entity as IDateAuditEntity;
            IUserAuditEntity<TUserAuditKey> userAuditEntity = entity as IUserAuditEntity<TUserAuditKey>;

            //Set the Concurrency Stamp
            if (concurrencyStampEntity != null)
                concurrencyStampEntity.ConcurrencyStamp = Guid.NewGuid().ToString();

            //Check if this entity is in the context, i.e. is it new
            if (entity.Id.Equals(default(TEntityKey)) && (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached)))
            {
                if (datedEntity != null)
                    datedEntity.CreatedDate = DateTime.UtcNow;

                if (userAuditEntity != null)
                    userAuditEntity.CreatedById = m_UserAuditDataFactory.CurrentUserId;

                if (addToContext)
                    Context.Set<TEntity>().Add(entity);
            }

            if (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached) || dbEntity.State.HasFlag(EntityState.Modified))
            {
                if (datedEntity != null)
                    datedEntity.UpdatedDate = DateTime.UtcNow;

                if (userAuditEntity != null)
                    userAuditEntity.UpdatedById = m_UserAuditDataFactory.CurrentUserId;
            }
        }

        protected virtual void LogDbEntityValidationExceptionDetails(DbEntityValidationException exc)
        {
            foreach (var item in exc.EntityValidationErrors)
            {
                string entityType = item.Entry.Entity.GetType().BaseType.FullName;
                
                Dictionary<string, object> currentValues = item.Entry.CurrentValues.PropertyNames.ToDictionary(x => x, x => item.Entry.CurrentValues.GetValue<object>(x));
                Dictionary<string, object> originalValues = null;

                //Can only get the OriginalValues if the entity has been modified from a previously persisted version.
                if(item.Entry.State.HasFlag(EntityState.Modified))
                    originalValues = item.Entry.OriginalValues.PropertyNames.ToDictionary(x => x, x => item.Entry.OriginalValues.GetValue<object>(x));

                Log.WriteError(exc, new { entityType, item.IsValid, item.ValidationErrors, originalValues, currentValues, state = item.Entry.State.ToString() });
            }
        }

        /// <summary>
        /// Save All entities in a single transaction
        /// </summary>
        /// <param name="entities">The entities to be saved in a single transaction</param>
        /// <param name="bypassSaveLogic">This should almost always be set to true - you should never have to bypass the default logic except in exceptional cases! When bypassing, you'll have to do the work yourself!</param>
        public virtual void SaveAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                Guard.ArgumentNotNull(entities, nameof(entities));

                //Save all changes - do not push to the database yet
                if (!bypassSaveLogic)
                {
                    foreach (TEntity entity in entities)
                    {
                        Save(entity, false, true, options, childOptions);
                    }
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    Context.SaveChanges();

                    //Additional process after all changes have been committed to the database
                    AfterContextSavedChangesMultiple(entities, options ?? s_DefaultRepoOptions, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, options, childOptions }, "Bulk Save Concurrency Exception", true))
            {
                throw new DataAccessConcurrencyException(c_BulkActionConcurrencyExceptionErrorMessage, exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, options, childOptions }))
            {
                throw;
            }
        }

        public virtual async Task SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guard.ArgumentNotNull(entities, nameof(entities));

                //Save all changes - do not push to the database yet
                if (!bypassSaveLogic)
                {
                    foreach (TEntity entity in entities)
                    {
                        await SaveAsync(entity, cancellationToken, false, true, options, childOptions);
                    }
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    await Context.SaveChangesAsync(cancellationToken);

                    //Additional process after all changes have been committed to the database
                    await AfterContextSavedChangesMultipleAsync(entities, cancellationToken, options ?? s_DefaultRepoOptions, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, options, childOptions }, "Bulk Save Concurrency Exception", true))
            {
                throw new DataAccessConcurrencyException(c_BulkActionConcurrencyExceptionErrorMessage, exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, options, childOptions }))
            {
                throw;
            }
        }
        #endregion

        #region Delete
        public virtual void Delete(TEntity entity, bool pushChangesToDb = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                Guard.ArgumentNotNull(entity, nameof(entity));

                //Ensure the default options are used when not explicitly provided.
                options = options ?? s_DefaultRepoOptions;

                BeforeContextDeleting(entity, options, childOptions);

                //Common work shared between the synchronous and asynchronous version of the Delete method
                PreDeleteWork(entity);

                AfterContextDeleting(entity, options, childOptions);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    Context.SaveChanges();

                    AfterContextDeletedChanges(entity, options, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, options, childOptions }, "Concurrency Exception for Id", true))
            {
                throw new DataAccessConcurrencyException(string.Format(c_ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, options, childOptions }, "Failed for Id"))
            {
                throw;
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guard.ArgumentNotNull(entity, nameof(entity));

                //Ensure the default options are used when not explicitly provided.
                options = options ?? s_DefaultRepoOptions;

                await BeforeContextDeletingAsync(entity, cancellationToken, options, childOptions);

                //Common work shared between the synchronous and asynchronous version of the Delete method
                PreDeleteWork(entity);

                await AfterContextDeletingAsync(entity, cancellationToken, options, childOptions);

                if (pushChangesToDb)
                {
                    //Push changes to the database
                    await Context.SaveChangesAsync(cancellationToken);

                    await AfterContextDeletedChangesAsync(entity, cancellationToken, options, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, options, childOptions }, "Concurrency Exception for Id", true))
            {
                throw new DataAccessConcurrencyException(string.Format(c_ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, options, childOptions }, "Failed for Id"))
            {
                throw;
            }
        }

        /// <summary>
        /// Delete all entities in a single transaction
        /// </summary>
        /// <param name="entities">The entities to be deleted</param>
        /// <param name="enableEntityValidation">Perform entity validation</param>
        public virtual void DeleteAll(IEnumerable<TEntity> entities, bool pushChangesToDb = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                Guard.ArgumentNotNull(entities, nameof(entities));

                foreach (TEntity entity in entities)
                {
                    Delete(entity, false, options, childOptions);
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    Context.SaveChanges();

                    AfterContextDeletedChangesMultiple(entities, options ?? s_DefaultRepoOptions, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, options, childOptions }, "Bulk Delete Concurrency Exception", true))
            {
                throw new DataAccessConcurrencyException(c_BulkActionConcurrencyExceptionErrorMessage, exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, options, childOptions }))
            {
                throw;
            }
        }

        protected virtual void PreDeleteWork(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public virtual async Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken), bool pushChangesToDb = true, TRepoOptions options = null, params RepoOptions[] childOptions)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guard.ArgumentNotNull(entities, nameof(entities));

                foreach (TEntity entity in entities)
                {
                    await DeleteAsync(entity, cancellationToken, false, options, childOptions);
                }

                if (pushChangesToDb)
                {
                    //Commit changes to the database as a single transaction
                    await Context.SaveChangesAsync(cancellationToken);

                    await AfterContextDeletedChangesMultipleAsync(entities, cancellationToken, options ?? s_DefaultRepoOptions, childOptions);
                }
            }
            catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, options, childOptions }, "Bulk Delete Concurrency Exception", true))
            {
                throw new DataAccessConcurrencyException(c_BulkActionConcurrencyExceptionErrorMessage, exc);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, options, childOptions }))
            {
                throw;
            }
        }

        protected virtual string FormatEntityIds(IEnumerable<TEntity> entities) => string.Join(",", entities.Select(x => x.Id));
        #endregion

        #region Protected Methods

        #region Events
        /// <summary>
        /// Overriding this method allows you to perform custom validation on the entity before its state on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        protected virtual void ValidateEntity(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform custom validation on the entity before its state on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        protected virtual Task ValidateEntityAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidateEntity(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void BeforeContextSaving(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task BeforeContextSavingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            BeforeContextSaving(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform work after the state of the entity on the database context has been affected but before
        /// the changes have been pushed to the database.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextSaving(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform work after the state of the entity on the database context has been affected but before
        /// the changes have been pushed to the database.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextSavingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextSaving(entity, options, childOptions);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChanges"/> has taken place.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextSavedChanges(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChangesAsync()"/> has taken place.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextSavedChangesAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextSavedChanges(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChanges"/> has taken place when working with multiple entities.
        /// </summary>
        /// <param name="entities">The entities</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextSavedChangesMultiple(IEnumerable<TEntity> entities, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChangesAsync()"/> has taken place when working with multiple entities.
        /// </summary>
        /// <param name="entities">The entities</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextSavedChangesMultipleAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextSavedChangesMultiple(entities, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void BeforeContextDeleting(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task BeforeContextDeletingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            BeforeContextDeleting(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform work after the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextDeleting(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform work after the state of the entity on the database context is affected.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextDeletingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextDeleting(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChanges"/> has taken place.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextDeletedChanges(TEntity entity, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChangesAsync"/> has taken place.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextDeletedChangesAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextDeletedChanges(entity, options, childOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChanges"/> has taken place when working with multiple entities.
        /// </summary>
        /// <param name="entities">The entities</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual void AfterContextDeletedChangesMultiple(IEnumerable<TEntity> entities, TRepoOptions options, RepoOptions[] childOptions)
        {
        }

        /// <summary>
        /// Overriding this method allows you to perform any work after the call to <see cref="DbContext.SaveChangesAsync"/> has taken place when working with multiple entities.
        /// </summary>
        /// <param name="entities">The entities</param>
        /// <param name="options">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
        protected virtual Task AfterContextDeletedChangesMultipleAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
        {
            AfterContextDeletedChangesMultiple(entities, options, childOptions);

            return Task.CompletedTask;
        }
        #endregion

        #region SyncDependencies
        protected void SyncDependencies<TTargetEntity, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, RepoOptions[] options)
            where TTargetEntity : class, IEntity
            where TTargetRepository : IGenericRepository<TTargetEntity>
        {
            SyncDependencies<TTargetEntity, int, TTargetRepository>(alteredColl, repository, func, options);
        }

        protected void SyncDependencies<TTargetEntity, TTargetEntityKey, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, RepoOptions[] options)
            where TTargetEntity : class, IEntity<TTargetEntityKey>
            where TTargetEntityKey : IEquatable<TTargetEntityKey>
            where TTargetRepository : IGenericRepository<TTargetEntity, TTargetEntityKey>
        {
            SyncDependencies<TTargetEntity, TTargetEntityKey, RepoOptions, TTargetRepository>(alteredColl, repository, func, options);
        }

        protected virtual void SyncDependencies<TTargetEntity, TTargetEntityKey, TTargetEntityRepoOptions, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, RepoOptions[] options)
            where TTargetEntity : class, IEntity<TTargetEntityKey>
            where TTargetEntityKey : IEquatable<TTargetEntityKey>
            where TTargetEntityRepoOptions : RepoOptions, new()
            where TTargetRepository : IGenericRepository<TTargetEntity, TTargetEntityKey, TTargetEntityRepoOptions>
        {
            //Copy the incoming list here - this is because the code in foreach declaration below finds all the entities matching the where clause
            //but the problem is that when that happens, the alteredColl parameter is a reference to the same underlying collection. This means
            //any items that have been removed from the incoming alteredColl will be added back to it. To get around this, we need to copy all the items from alteredColl
            //to a new List first to stop this from happening.
            alteredColl = new List<TTargetEntity>(alteredColl);

            //Find the RepoOptions for this repository if provided in the options collection
            TTargetEntityRepoOptions targetOptions = options?.OfType<TTargetEntityRepoOptions>().FirstOrDefault();

            //Ensure we have deleted the dependencies (children) we no longer need
            foreach (TTargetEntity entity in Context.Set<TTargetEntity>().Where(func))
            {
                if (!alteredColl.Contains(entity))
                {
                    //Delete the dependency, but do not push changes to the database
                    repository.Delete(entity, false, targetOptions, options);
                }
            }

            foreach (TTargetEntity entity in alteredColl)
            {
                //Look for the entity in the context - this action will allow us to determine it's state
                DbEntityEntry<TTargetEntity> dbEntity = Context.Entry(entity);

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
                    repository.Save(entity, false, false, targetOptions, options);
                }
            }
        }

        protected Task SyncDependenciesAsync<TTargetEntity, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, CancellationToken cancellationToken, RepoOptions[] options)
            where TTargetEntity : class, IEntity
            where TTargetRepository : IGenericRepository<TTargetEntity>
        {
            return SyncDependenciesAsync<TTargetEntity, int, TTargetRepository>(alteredColl, repository, func, cancellationToken, options);
        }

        protected Task SyncDependenciesAsync<TTargetEntity, TTargetEntityKey, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, CancellationToken cancellationToken, RepoOptions[] options)
            where TTargetEntity : class, IEntity<TTargetEntityKey>
            where TTargetEntityKey : IEquatable<TTargetEntityKey>
            where TTargetRepository : IGenericRepository<TTargetEntity, TTargetEntityKey>
        {
            return SyncDependenciesAsync<TTargetEntity, TTargetEntityKey, RepoOptions, TTargetRepository>(alteredColl, repository, func, cancellationToken, options);
        }

        protected virtual async Task SyncDependenciesAsync<TTargetEntity, TTargetEntityKey, TTargetEntityRepoOptions, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> func, CancellationToken cancellationToken, RepoOptions[] options)
            where TTargetEntity : class, IEntity<TTargetEntityKey>
            where TTargetEntityKey : IEquatable<TTargetEntityKey>
            where TTargetEntityRepoOptions : RepoOptions, new()
            where TTargetRepository : IGenericRepository<TTargetEntity, TTargetEntityKey, TTargetEntityRepoOptions>
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Copy the incoming list here - this is because the code in foreach declaration below finds all the entities matching the where clause
            //but the problem is that when that happens, the alteredColl parameter is a reference to the same underlying collection. This means
            //any items that have been removed from the incoming alteredColl will be added back to it. To get around this, we need to copy all the items from alteredColl
            //to a new List first to stop this from happening.
            alteredColl = new List<TTargetEntity>(alteredColl);

            //Find the RepoOptions for this repository if provided in the options collection
            TTargetEntityRepoOptions targetOptions = options?.OfType<TTargetEntityRepoOptions>().FirstOrDefault();

            //Ensure we have deleted the dependencies (children) we no longer need
            foreach (TTargetEntity entity in Context.Set<TTargetEntity>().Where(func))
            {
                if (!alteredColl.Contains(entity))
                {
                    //Delete the dependency, but do not push changes to the database
                    await repository.DeleteAsync(entity, cancellationToken, false, targetOptions, options);
                }
            }

            foreach (TTargetEntity entity in alteredColl)
            {
                //Look for the entity in the context - this action will allow us to determine it's state
                DbEntityEntry<TTargetEntity> dbEntity = Context.Entry(entity);

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
                    await repository.SaveAsync(entity, cancellationToken, false, false, targetOptions, options);
                }
            }
        }
        #endregion

        #region Validation

        protected virtual void ValidatePropertyStringLength(string value, string propertyName, int minLength, int maxLength, bool required = true)
        {
            if (!value.IsValidLength(minLength, maxLength, !required))
                throw new DataAccessValidationException(string.Format(c_InvalidPropertyStringLengthErrorMessageFormat, propertyName, minLength, maxLength));
        }

        protected virtual void ValidatePropertyNumberRange<TProperty>(TProperty? value, string propertyName, TProperty min, TProperty max, bool required = true)
            where TProperty : struct, IComparable<TProperty>
        {
            if (!value.IsValidRange(min, max, !required))
                throw new DataAccessValidationException(string.Format(c_InvalidPropertyNumberRangeErrorMessageFormat, propertyName, min, max));
        }
        #endregion

        #region Sanitize Methods
        protected virtual bool IsEmptyEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        protected virtual Task<bool> IsEmptyEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual void SanitizeEntity(TEntity entity)
        {
        }

        protected virtual Task SanitizeEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }
        #endregion

        #endregion

        #region Public Methods
        public virtual void RemoveEmptyEntities(ICollection<TEntity> entities)
        {
            List<TEntity> lstToRemove = entities.Where(x => IsEmptyEntity(x)).ToList();

            foreach (TEntity entity in lstToRemove)
            {
                entities.Remove(entity);
                
                //Make sure it is removed from the Context if it has just been added - make it detached
                DbEntityEntry<TEntity> dbEntityEntry = Context.Entry(entity);

                if (dbEntityEntry.State == EntityState.Added)
                    dbEntityEntry.State = EntityState.Detached;
            }
        }

        public virtual async Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<TEntity> lstToRemove = new List<TEntity>();

            foreach(TEntity entity in entities)
            {
                if (await IsEmptyEntityAsync(entity, cancellationToken))
                    lstToRemove.Add(entity);
            }

            foreach (TEntity entity in lstToRemove)
            {
                entities.Remove(entity);

                //Make sure it is removed from the Context if it has just been added - make it detached
                DbEntityEntry<TEntity> dbEntityEntry = Context.Entry(entity);

                if (dbEntityEntry.State == EntityState.Added)
                    dbEntityEntry.State = EntityState.Detached;
            }
        }

        public virtual List<TEntity> FindAll(bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                return Items.TrackChanges(trackChanges).IncludeMap(map).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.TrackChanges(trackChanges).IncludeMap(map).ToListAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual List<TEntity> FindAllByIdList(IEnumerable<TEntityKey> ids, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                return Items.TrackChanges(trackChanges).IncludeMap(map).Where(x => ids.Contains(x.Id)).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public virtual Task<List<TEntity>> FindAllByIdListAsync(IEnumerable<TEntityKey> ids, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.TrackChanges(trackChanges).IncludeMap(map).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc, ids))
            {
                throw;
            }
        }

        public virtual TEntity FindById(TEntityKey id, bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                return Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefault(x => x.Id.Equals(id));
            }
            catch (Exception exc) when (Log.WriteError(exc, id))
            {
                throw;
            }
        }

        public virtual Task<TEntity> FindByIdAsync(TEntityKey id, CancellationToken cancellationToken = default(CancellationToken), bool trackChanges = false, IncludeMap<TEntity> map = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.TrackChanges(trackChanges).IncludeMap(map).SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc, id))
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
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public virtual Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Items.CountAsync(cancellationToken);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}