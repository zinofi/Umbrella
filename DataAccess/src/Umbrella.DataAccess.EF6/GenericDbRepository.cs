using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.Utilities;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.DataAccess.EF6
{
	/// <summary>
	/// Serves as the base class for repositories which provide CRUD access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	public abstract class GenericDbRepository<TEntity, TDbContext> : GenericDbRepository<TEntity, TDbContext, RepoOptions>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbRepository{TEntity, TDbContext}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		/// <param name="entityValidator">The entity validator.</param>
		public GenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor,
			IUmbrellaDbContextHelper dbContextHelper,
			IEntityValidator entityValidator)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor, dbContextHelper, entityValidator)
		{
		}
	}

	/// <summary>
	/// Serves as the base class for repositories which provide CRUD access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	public abstract class GenericDbRepository<TEntity, TDbContext, TRepoOptions> : GenericDbRepository<TEntity, TDbContext, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbRepository{TEntity, TDbContext, TRepoOptions}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		/// <param name="entityValidator">The entity validator.</param>
		public GenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor,
			IUmbrellaDbContextHelper dbContextHelper,
			IEntityValidator entityValidator)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor, dbContextHelper, entityValidator)
		{
		}
	}

	/// <summary>
	/// Serves as the base class for repositories which provide CRUD access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	public abstract class GenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : GenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		/// <param name="entityValidator">The entity validator.</param>
		public GenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<int> currentUserIdAccessor,
			IUmbrellaDbContextHelper dbContextHelper,
			IEntityValidator entityValidator)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor, dbContextHelper, entityValidator)
		{
		}
	}

	/// <summary>
	/// Serves as the base class for repositories which provide CRUD access to entities stored in a database accessed using Entity Framework 6.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TUserAuditKey">The type of the user audit key.</typeparam>
	public abstract class GenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : ReadOnlyGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey>, IGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
		where TUserAuditKey : IEquatable<TUserAuditKey>
	{
		#region Protected Properties
		/// <summary>
		/// Gets the database context helper.
		/// </summary>
		protected IUmbrellaDbContextHelper DbContextHelper { get; }

		/// <summary>
		/// Gets the entity validator.
		/// </summary>
		protected IEntityValidator EntityValidator { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		/// <param name="entityValidator">The entity validator.</param>
		public GenericDbRepository(
			TDbContext dbContext,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			ICurrentUserIdAccessor<TUserAuditKey> currentUserIdAccessor,
			IUmbrellaDbContextHelper dbContextHelper,
			IEntityValidator entityValidator)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor)
		{
			DbContextHelper = dbContextHelper;
			EntityValidator = entityValidator;
		}
		#endregion

		#region Save
		/// <inheritdoc />
		public virtual async Task<SaveResult<TEntity>> SaveAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null, bool forceAdd = false)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(entity, nameof(entity));

			try
			{
				await ThrowIfCannotAcesssAsync(entity, cancellationToken).ConfigureAwait(false);
				ThrowIfConcurrencyTokenMismatch(entity);

				// Ensure the default options are used when not explicitly provided.
				repoOptions ??= DefaultRepoOptions;

				if (repoOptions.SanitizeEntity)
					await SanitizeEntityAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				// Additional processing before changes have been reflected in the database context
				await BeforeContextSavingAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				if (repoOptions.ValidateEntity)
				{
					ValidationResult[] lstValidationResult = await ValidateEntityAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

					return new SaveResult<TEntity>(false, entity, lstValidationResult);
				}

				// Common work
				PreSaveWork(entity, addToContext, forceAdd, out bool isNew);

				// Additional processing after changes have been reflected in the database context but not yet pushed to the database
				await AfterContextSavingAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				DbContextHelper.RegisterPostSaveChangesAction(entity, (cancellationToken) => AfterContextSavedChangesAsync(entity, isNew, cancellationToken, repoOptions, childOptions));

				if (pushChangesToDb)
					await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return new SaveResult<TEntity>(true, entity);
			}
			catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, repoOptions, childOptions }, "Concurrency Exception for Id", returnValue: true))
			{
				throw new UmbrellaDataAccessConcurrencyException(string.Format(ErrorMessages.ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
			}
			catch (DbEntityValidationException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, repoOptions, childOptions }, "Data Validation Exception for Id", returnValue: true))
			{
				try
				{
					LogDbEntityValidationExceptionDetails(exc);

					return CreateSaveResultFromDbEntityValidationException(exc);
				}
				catch (Exception excInner)
				{
					throw new UmbrellaDataAccessAggregateException($"Initally, an exception of type {nameof(DbEntityValidationException)} was encountered. However, whilst trying to evaluate this exception, another exception was encountered.", exc, excInner);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, addToContext, repoOptions, childOptions }, "Failed for Id", returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem saving the entity.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IReadOnlyCollection<SaveResult<TEntity>>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(entities, nameof(entities));

			try
			{
				// Save all changes - do not push to the database yet
				if (!bypassSaveLogic)
				{
					var lstSaveResult = new List<SaveResult<TEntity>>();

					foreach (TEntity entity in entities)
					{
						SaveResult<TEntity> saveResult = await SaveAsync(entity, cancellationToken, false, true, repoOptions, childOptions).ConfigureAwait(false);
						lstSaveResult.Add(saveResult);
					}

					return lstSaveResult;
				}

				if (pushChangesToDb)
					await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return entities.Select(x => new SaveResult<TEntity>(true, x)).ToArray();
			}
			catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, repoOptions, childOptions }, "Bulk Save Concurrency Exception", returnValue: true))
			{
				throw new UmbrellaDataAccessConcurrencyException(ErrorMessages.BulkActionConcurrencyExceptionErrorMessage, exc);
			}
			catch (DbEntityValidationException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, repoOptions, childOptions }, "Data Validation Exception for Ids", returnValue: true))
			{
				try
				{
					LogDbEntityValidationExceptionDetails(exc);

					return CreateSaveResultsFromDbEntityValidationException(exc);
				}
				catch (Exception excInner)
				{
					throw new UmbrellaDataAccessAggregateException($"Initally, an exception of type {nameof(DbEntityValidationException)} was encountered. However, whilst trying to evaluate this exception, another exception was encountered.", exc, excInner);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, bypassSaveLogic, repoOptions, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem saving the specified entities.", exc);
			}
		}
		#endregion

		#region Delete
		/// <inheritdoc />
		public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(entity, nameof(entity));

			try
			{
				await ThrowIfCannotAcesssAsync(entity, cancellationToken).ConfigureAwait(false);
				ThrowIfConcurrencyTokenMismatch(entity);

				// Ensure the default options are used when not explicitly provided.
				repoOptions ??= DefaultRepoOptions;

				await BeforeContextDeletingAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				Context.Set<TEntity>().Remove(entity);
				Context.Entry(entity).State = EntityState.Deleted;

				await AfterContextDeletingAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false);

				DbContextHelper.RegisterPostSaveChangesAction(entity, (cancellationToken) => AfterContextDeletedChangesAsync(entity, cancellationToken, repoOptions, childOptions));

				if (pushChangesToDb)
					await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, repoOptions, childOptions }, "Concurrency Exception for Id", returnValue: true))
			{
				throw new UmbrellaDataAccessConcurrencyException(string.Format(ErrorMessages.ConcurrencyExceptionErrorMessageFormat, entity.Id), exc);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { entity.Id, pushChangesToDb, repoOptions, childOptions }, "Failed for Id", returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the specified entity.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(entities, nameof(entities));

			try
			{
				foreach (TEntity entity in entities)
				{
					await DeleteAsync(entity, cancellationToken, false, repoOptions, childOptions).ConfigureAwait(false);
				}

				if (pushChangesToDb)
					await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (DbUpdateConcurrencyException exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, repoOptions, childOptions }, "Bulk Delete Concurrency Exception", returnValue: true))
			{
				throw new UmbrellaDataAccessConcurrencyException(ErrorMessages.BulkActionConcurrencyExceptionErrorMessage, exc);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ids = FormatEntityIds(entities), pushChangesToDb, repoOptions, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the specified entities.", exc);
			}
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Determines whether the concurrency token assigned on the entity matches the one stored on the tracking entry in the database context.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		///   <see langword="true"/> if it matches; otherwise, <see langword="false"/>.
		/// </returns>
		protected bool IsConcurrencyTokenMismatch(TEntity entity)
			=> !entity.Id.Equals(default) && entity is IConcurrencyStamp concurrencyStampEntity && Context.Entry(concurrencyStampEntity).Property(x => x.ConcurrencyStamp).OriginalValue != concurrencyStampEntity.ConcurrencyStamp;

		/// <summary>
		/// Throws an exception if there is a concurrency token mismatch.
		/// </summary>
		/// <param name="entity">The entity.</param>
		protected void ThrowIfConcurrencyTokenMismatch(TEntity entity)
		{
			if (IsConcurrencyTokenMismatch(entity))
				throw new UmbrellaDataAccessConcurrencyException(string.Format(ErrorMessages.ConcurrencyExceptionErrorMessageFormat, entity.Id));
		}

		/// <summary>
		/// Formats the entity ids as comma-delimited string. This is only used for logging purposes to record the ids.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <returns>A comma-delimited string of entity ids.</returns>
		protected virtual string FormatEntityIds(IEnumerable<TEntity> entities) => string.Join(",", entities.Select(x => x.Id));

		/// <summary>
		/// Performs save operations common to all Save methods. 
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="addToContext">if set to <c>true</c>, adds the entity to the context.</param>
		/// <param name="forceAdd">Forces the entity to be added to the context in the <see cref="EntityState.Added"/> state.</param>
		/// <param name="isNew">if set to <c>true</c>, specifies that the entity is new.</param>
		protected virtual void PreSaveWork(TEntity entity, bool addToContext, bool forceAdd, out bool isNew)
		{
			// Assume the entity is not new initially
			isNew = false;

			// Look for the entity in the context - this action will allow us to determine it's state
			DbEntityEntry<TEntity> dbEntity = Context.Entry(entity);

			// Set the Concurrency Stamp
			if (entity is IConcurrencyStamp concurrencyStampEntity)
				concurrencyStampEntity.ConcurrencyStamp = Guid.NewGuid().ToString();

			// Check if this entity is in the context, i.e. is it new
			if (forceAdd || (entity.Id.Equals(default) && (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached))))
			{
				isNew = true;

				if (entity is ICreatedDateAuditEntity dateAuditEntity)
					dateAuditEntity.CreatedDate = DateTime.UtcNow;

				if (entity is ICreatedUserAuditEntity<TUserAuditKey> userAuditEntity)
					userAuditEntity.CreatedById = CurrentUserId;

				if (addToContext)
					Context.Set<TEntity>().Add(entity);
			}

			if (dbEntity.State.HasFlag(EntityState.Added) || dbEntity.State.HasFlag(EntityState.Detached) || dbEntity.State.HasFlag(EntityState.Modified))
			{
				if (entity is IUpdatedDateAuditEntity dateAuditEntity)
					dateAuditEntity.UpdatedDate = DateTime.UtcNow;

				if (entity is IUpdatedUserAuditEntity<TUserAuditKey> userAuditEntity)
					userAuditEntity.UpdatedById = CurrentUserId;
			}
		}

		/// <summary>
		/// Logs the database entity validation exception details.
		/// </summary>
		/// <param name="exc">The exception.</param>
		protected virtual void LogDbEntityValidationExceptionDetails(DbEntityValidationException exc)
		{
			foreach (var item in exc.EntityValidationErrors)
			{
				string entityType = item.Entry.Entity.GetType().BaseType.FullName;

				var currentValues = item.Entry.CurrentValues.PropertyNames.ToDictionary(x => x, x => item.Entry.CurrentValues.GetValue<object>(x));
				Dictionary<string, object> originalValues = null;

				// Can only get the OriginalValues if the entity has been modified from a previously persisted version.
				if (item.Entry.State.HasFlag(EntityState.Modified))
					originalValues = item.Entry.OriginalValues.PropertyNames.ToDictionary(x => x, x => item.Entry.OriginalValues.GetValue<object>(x));

				Log.WriteError(exc, new { entityType, item.IsValid, item.ValidationErrors, originalValues, currentValues, state = item.Entry.State.ToString() });
			}
		}

		/// <summary>
		/// Creates a save result from the database entity validation exception.
		/// </summary>
		/// <param name="exc">The exception.</param>
		/// <returns>The SaveResult.</returns>
		protected virtual SaveResult<TEntity> CreateSaveResultFromDbEntityValidationException(DbEntityValidationException exc)
					=> CreateSaveResultsFromDbEntityValidationException(exc).Single();

		/// <summary>
		/// Creates the save results from the database entity validation exception.
		/// </summary>
		/// <param name="exc">The exception.</param>
		/// <returns>The SaveResult collection.</returns>
		protected virtual List<SaveResult<TEntity>> CreateSaveResultsFromDbEntityValidationException(DbEntityValidationException exc)
		{
			var lstSaveResult = new List<SaveResult<TEntity>>();

			foreach (var item in exc.EntityValidationErrors)
			{
				if (item.Entry.Entity is TEntity entity)
				{
					var lstValidationResult = item.ValidationErrors.GroupBy(x => x.ErrorMessage).Select(x => new ValidationResult(x.Key, x.Select(y => y.PropertyName)));

					var saveResult = new SaveResult<TEntity>(false, entity, lstValidationResult);
					lstSaveResult.Add(saveResult);
				}
			}

			return lstSaveResult;
		}

		#region Events		
		/// <summary>
		/// Overriding this method allows you to perform custom validation on the entity before its state on the database context is affected.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task<ValidationResult[]> ValidateEntityAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(Array.Empty<ValidationResult>());
		}

		/// <summary>
		/// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task BeforeContextSavingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform work after the state of the entity on the database context has been affected but before
		/// the changes have been pushed to the database.
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task AfterContextSavingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform any work after the call to <see cref="UmbrellaDbContext.SaveChangesAsync()"/> has taken place.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="isNew">Specifies if the entity is new.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task AfterContextSavedChangesAsync(TEntity entity, bool isNew, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform work before the state of the entity on the database context is affected.
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task BeforeContextDeletingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform work after the state of the entity on the database context is affected.
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task AfterContextDeletingAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

#pragma warning disable CS0419 // Ambiguous reference in cref attribute
		/// <summary>
		/// Overriding this method allows you to perform any work after the call to either <see cref="UmbrellaDbContext.SaveChangesAsync"/> or <see cref="UmbrellaDbContext.SaveChangesAsync(CancellationToken)"/> has taken place.
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options. If not overridden with a different generic type parameter, the default of <see cref="RepoOptions"/> is used. This parameter will never be null.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task AfterContextDeletedChangesAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}
		#endregion

		#region SyncDependencies		
		/// <summary>
		/// Synchronizes the dependencies of the current entity.
		/// For example, the entity may have a collection property, e.g. Person has a list of Pets and Pet has a foreign key of PersonId so
		/// there is a 1-N relationship between Person and Pet. This method would be called as: SyncDependenciesAsync(person.Pets, petRepo, x => x.PersonId == person.Id, cancellationToken, options).
		/// <para>This method should be used with caution as it has the overhead of loading all of the child entities that match the predicate, e.g. all Pets for the person.Id. For large collections this is not practical so use with care as alternative solutions will likely be more performant.</para>
		/// </summary>
		/// <typeparam name="TTargetEntity">The type of the target entity.</typeparam>
		/// <typeparam name="TTargetEntityRepoOptions">The type of the target entity repo options.</typeparam>
		/// <typeparam name="TTargetEntityKey">The type of the target entity key.</typeparam>
		/// <typeparam name="TTargetRepository">The type of the target repository.</typeparam>
		/// <param name="alteredColl">The altered collection.</param>
		/// <param name="repository">The target repository.</param>
		/// <param name="predicate">The predicate used to filter .</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="options">The collection of repo options. This can be used by this method to determine if the synchronization logic should be executed.</param>
		/// <returns>An awaitable <see cref="Task" /> which will complete once all work has been completed.</returns>
		protected virtual async Task SyncDependenciesAsync<TTargetEntity, TTargetEntityRepoOptions, TTargetEntityKey, TTargetRepository>(ICollection<TTargetEntity> alteredColl, TTargetRepository repository, Expression<Func<TTargetEntity, bool>> predicate, CancellationToken cancellationToken, IEnumerable<RepoOptions> options)
					where TTargetEntity : class, IEntity<TTargetEntityKey>
					where TTargetEntityKey : IEquatable<TTargetEntityKey>
					where TTargetEntityRepoOptions : RepoOptions, new()
					where TTargetRepository : IGenericDbRepository<TTargetEntity, TTargetEntityRepoOptions, TTargetEntityKey>
		{
			cancellationToken.ThrowIfCancellationRequested();

			// Find the RepoOptions for this repository if provided in the options collection
			TTargetEntityRepoOptions targetOptions = options.OfType<TTargetEntityRepoOptions>().FirstOrDefault();

			// If the options specify that children should not be processed here, abort.
			if (!targetOptions.ProcessChildren)
				return;

			// Copy the incoming list here - this is because the code in the foreach declaration below finds all the entities matching the where clause
			// but the problem is that when that happens, the alteredColl parameter is a reference to the same underlying collection. This means
			// any items that have been removed from the incoming alteredColl will be added back to it. To get around this, we need to copy all the items from alteredColl
			// to a new List first to stop this from happening.
			alteredColl = new List<TTargetEntity>(alteredColl);

			// Ensure we have deleted the dependencies (children) we no longer need
			foreach (TTargetEntity entity in Context.Set<TTargetEntity>().Where(predicate))
			{
				if (!alteredColl.Contains(entity))
				{
					// Delete the dependency, but do not push changes to the database
					await repository.DeleteAsync(entity, cancellationToken, false, targetOptions, options).ConfigureAwait(false);
				}
			}

			foreach (TTargetEntity entity in alteredColl)
			{
				// Look for the entity in the context - this action will allow us to determine it's state
				DbEntityEntry<TTargetEntity> dbEntity = Context.Entry(entity);

				// Determine entities that have been added or modified - in these cases we need to call Save so that any custom
				// repository logic is executed
				if (dbEntity.State.HasFlag(EntityState.Detached)
					|| dbEntity.State.HasFlag(EntityState.Added)
					|| dbEntity.State.HasFlag(EntityState.Modified)
					|| dbEntity.State.HasFlag(EntityState.Unchanged))
				{
					// Do not add children to the context at this point. This still allows us to perform our save
					// logic on the entity, but it also means that should something go wrong that means
					// persisting the parent entity is not valid, we don't end up in a situation where we have
					// child objects as part of the context that shouldn't be saved.
					await repository.SaveAsync(entity, cancellationToken, false, false, targetOptions, options).ConfigureAwait(false);
				}
			}
		}
		#endregion

		#region Sanitize Methods
		/// <summary>
		/// Determines whether the specified entity is empty.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns><see langword="true" /> if it is empty, otherwise <see langword="false" /></returns>
		protected virtual Task<bool> IsEmptyEntityAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(false);
		}

		/// <summary>
		/// Sanitizes the entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>A task to await sanitization of the entity.</returns>
		protected virtual Task SanitizeEntityAsync(TEntity entity, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}
		#endregion

		#endregion

		#region Public Methods
		/// <inheritdoc />
		public virtual async Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken, TRepoOptions repoOptions, IEnumerable<RepoOptions> childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var lstToRemove = new List<TEntity>();

			foreach (TEntity entity in entities)
			{
				if (await IsEmptyEntityAsync(entity, cancellationToken, repoOptions, childOptions).ConfigureAwait(false))
					lstToRemove.Add(entity);
			}

			foreach (TEntity entity in lstToRemove)
			{
				entities.Remove(entity);

				// Make sure it is removed from the Context if it has just been added - make it detached
				DbEntityEntry<TEntity> dbEntityEntry = Context.Entry(entity);

				if (dbEntityEntry.State == EntityState.Added)
					dbEntityEntry.State = EntityState.Detached;
			}
		}
		#endregion
	}
}