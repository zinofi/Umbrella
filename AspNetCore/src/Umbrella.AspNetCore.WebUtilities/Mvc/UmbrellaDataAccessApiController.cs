// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Shared.Models;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Umbrella.AspNetCore.WebUtilities.Mvc.Extensions;
using Umbrella.AspNetCore.WebUtilities.Mvc.Options;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.Utilities.Data.Concurrency;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	public abstract class UmbrellaDataAccessApiController : UmbrellaApiController
	{
		protected virtual string ConcurrencyErrorMessage { get; } = "This information has been changed elsewhere since this screen was loaded. Please refresh the screen and try again.";

		private protected UmbrellaDataAccessApiControllerOptions Options { get; }
		protected IMapper Mapper { get; }
		protected IAuthorizationService AuthorizationService { get; }
		protected ISynchronizationManager SynchronizationManager { get; }

		protected UmbrellaDataAccessApiController(
			ILogger logger,
			IWebHostEnvironment hostingEnvironment,
			UmbrellaDataAccessApiControllerOptions options,
			IMapper mapper,
			IAuthorizationService authorizationService,
			ISynchronizationManager synchronizationManager)
			: base(logger, hostingEnvironment)
		{
			Options = options;
			Mapper = mapper;
			AuthorizationService = authorizationService;
			SynchronizationManager = synchronizationManager;
		}

		protected virtual async Task<IActionResult> ReadAllAsync<TEntityResult, TEntity, TEntityKey, TRepositoryOptions, TItemModel, TPaginatedItemModel>(
			int pageNumber,
			int pageSize,
			SortExpression<TEntityResult>[]? sorters,
			FilterExpression<TEntity>[]? filters,
			FilterExpressionCombinator? filterCombinator,
			CancellationToken cancellationToken,
			Func<int, int, CancellationToken, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, TRepositoryOptions?, Task<PaginatedResultModel<TEntityResult>>> loadReadAllDataAsyncDelegate,
			Func<IReadOnlyCollection<TEntityResult>, TItemModel[]>? mapReadAllEntitiesDelegate = null,
			Func<PaginatedResultModel<TEntityResult>, TPaginatedItemModel, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, CancellationToken, Task>? afterCreateSearchSlimPaginatedModelAsyncDelegate = null,
			Func<TEntityResult, TItemModel, CancellationToken, Task<IActionResult?>>? afterCreateSlimModelAsyncDelegate = null,
			TRepositoryOptions? options = null,
			bool enableAuthorizationChecks = true)
			where TEntityResult : class, IEntity<TEntityKey>
			where TEntity : class, IEntity<TEntityKey>
			where TEntityKey : IEquatable<TEntityKey>
			where TRepositoryOptions : RepoOptions, new()
			where TPaginatedItemModel : PaginatedResultModel<TItemModel>, new()
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				ClampPaginationParameters(ref pageNumber, ref pageSize);

				PaginatedResultModel<TEntityResult> result = await loadReadAllDataAsyncDelegate(pageNumber, pageSize, cancellationToken, sorters, filters, filterCombinator, options).ConfigureAwait(false);

				// Authorization Checks
				if (enableAuthorizationChecks)
				{
					bool authorized = await AuthorizationService.AuthorizeAllAsync(User, result.Items, Options.ReadPolicyName, cancellationToken).ConfigureAwait(false);

					if (!authorized)
						return Forbidden("There are items that are forbidden from being accessed in the results.");
				}

				var model = new TPaginatedItemModel
				{
					Items = mapReadAllEntitiesDelegate is null ? Mapper.Map<TItemModel[]>(result.Items) : mapReadAllEntitiesDelegate(result.Items),
					PageNumber = pageNumber,
					PageSize = pageSize,
					TotalCount = result.TotalCount,
					MoreItems = pageNumber * pageSize < result.TotalCount
				};

				if (afterCreateSearchSlimPaginatedModelAsyncDelegate != null)
					await afterCreateSearchSlimPaginatedModelAsyncDelegate(result, model, sorters, filters, filterCombinator, cancellationToken).ConfigureAwait(false);

				if (afterCreateSlimModelAsyncDelegate != null)
				{
					for (int i = 0; i < model.Items.Count; i++)
					{
						IActionResult? actionResult = await afterCreateSlimModelAsyncDelegate(result.Items.ElementAt(i), model.Items.ElementAt(i), cancellationToken).ConfigureAwait(false);

						if (actionResult != null)
							return actionResult;
					}
				}

				return Ok(model);
			}
			catch (Exception exc) when (Options.ReadAllExceptionFilter(exc))
			{
				IActionResult? result = await Options.HandleReadAllExceptionAsync(exc);

				if (result != null)
					return result;

				throw;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, sorters = sorters?.ToSortExpressionDescriptors(), filters = filters?.ToFilterExpressionDescriptors() }, returnValue: !IsDevelopment))
			{
				return InternalServerError("An error has occurred whilst trying to get the list of items.");
			}
		}

		protected async Task<IActionResult> ReadAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel>(
			TEntityKey id,
			Lazy<TRepository> repository,
			CancellationToken cancellationToken,
			Func<TEntity, TModel>? mapperCallback = null,
			Func<TEntity, TModel, Task<IActionResult?>>? afterReadEntityCallback = null,
			bool trackChanges = false,
			IncludeMap<TEntity>? map = null,
			TRepositoryOptions? options = null,
			bool enableAuthorizationChecks = true,
			bool synchronizeAccess = false)
			where TEntity : class, IEntity<TEntityKey>
			where TEntityKey : IEquatable<TEntityKey>
			where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
			where TRepositoryOptions : RepoOptions, new()
		{
			cancellationToken.ThrowIfCancellationRequested();

			ISynchronizationRoot? syncRoot = null;

			try
			{
				if (synchronizeAccess && id.ToString() is string syncKey)
					syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

				TEntity? item = await repository.Value.FindByIdAsync(id, cancellationToken, trackChanges, map, options).ConfigureAwait(false);

				if (item is null)
					return NotFound("The item could not be found. Please go back to the listing screen and try again.");

				// Ensure the current user has Read permissions.
				if (enableAuthorizationChecks)
				{
					AuthorizationResult authResult = await AuthorizationService.AuthorizeAsync(User, item, Options.ReadPolicyName).ConfigureAwait(false);

					if (!authResult.Succeeded)
						return Forbidden("You do not have permission to access the specified item.");
				}

				TModel model = mapperCallback is null
					? Mapper.Map<TModel>(item)
					: mapperCallback(item);

				if (afterReadEntityCallback != null)
				{
					IActionResult? result = await afterReadEntityCallback(item, model).ConfigureAwait(false);

					if (result != null)
						return result;
				}

				return Ok(model);
			}
			catch (Exception exc) when (Options.ReadExceptionFilter(exc))
			{
				IActionResult? result = await Options.HandleReadExceptionAsync(exc);

				if (result != null)
					return result;

				throw;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id }, returnValue: !IsDevelopment))
			{
				return InternalServerError("There has been a problem getting the specified item.");
			}
			finally
			{
				if (syncRoot != null)
					await syncRoot.DisposeAsync().ConfigureAwait(false);
			}
		}

		protected async Task<IActionResult> CreateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel, TResultModel>(
			TModel model,
			Lazy<TRepository> repository,
			CancellationToken cancellationToken,
			Func<TModel, TEntity>? mapperInputCallback = null,
			Func<TEntity, Task<IActionResult?>>? beforeCreateEntityCallback = null,
			Func<TEntity, TResultModel>? mapperOutputCallback = null,
			Func<TEntity, TResultModel, Task>? afterCreateEntityCallback = null,
			TRepositoryOptions? options = null,
			bool enableAuthorizationChecks = true,
			bool synchronizeAccess = false,
			bool enableOutputMapping = true)
			where TEntity : class, IEntity<TEntityKey>
			where TEntityKey : IEquatable<TEntityKey>
			where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
			where TRepositoryOptions : RepoOptions, new()
			where TResultModel : ICreateResultModel<TEntityKey>, new()
		{
			cancellationToken.ThrowIfCancellationRequested();

			ISynchronizationRoot? syncRoot = null;

			try
			{
				if (synchronizeAccess)
				{
					(Type type, string key)? syncKey = GetCreateSynchronizationRootKey(model);

					if (syncKey.HasValue)
						syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync(syncKey.Value.type, syncKey.Value.key, cancellationToken).ConfigureAwait(false);
				}

				var entity = mapperInputCallback is null
					? Mapper.Map<TEntity>(model)
					: mapperInputCallback(model);

				if (beforeCreateEntityCallback != null)
				{
					IActionResult? beforeResult = await beforeCreateEntityCallback(entity).ConfigureAwait(false);

					if (beforeResult != null)
						return beforeResult;
				}

				// Ensure the current user has Create permissions.
				if (enableAuthorizationChecks)
				{
					AuthorizationResult authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.CreatePolicyName).ConfigureAwait(false);

					if (!authResult.Succeeded)
						return Forbidden("You do not have permission to access the specified item.");
				}

				SaveResult<TEntity> saveResult = await repository.Value.SaveAsync(entity, cancellationToken, repoOptions: options).ConfigureAwait(false);

				if (saveResult.Success)
				{
					TResultModel result = default;

					if (enableOutputMapping)
					{
						result = mapperOutputCallback is null
							? Mapper.Map<TResultModel>(entity)
							: mapperOutputCallback(entity);
					}
					else
					{
						result = new TResultModel { Id = entity.Id };

						if (result is IConcurrencyStamp concurrencyStampResult && entity is IConcurrencyStamp concurrencyStampEntity)
							concurrencyStampResult.ConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
					}

					if (afterCreateEntityCallback != null)
						await afterCreateEntityCallback(entity, result).ConfigureAwait(false);

					return Created(result);
				}

				if (saveResult.ValidationResults?.Count > 0)
					return ValidationProblem(saveResult.ValidationResults.ToModelStateDictionary());

				return BadRequest("There was a problem saving the item. Please try again.");
			}
			catch (Exception exc) when (Options.CreateExceptionFilter(exc))
			{
				IActionResult? result = await Options.HandleCreateExceptionAsync(exc);

				if (result != null)
					return result;

				throw;
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: !IsDevelopment))
			{
				return InternalServerError("There has been a problem creating the specified item.");
			}
			finally
			{
				if (syncRoot != null)
					await syncRoot.DisposeAsync().ConfigureAwait(false);
			}
		}

		protected async Task<IActionResult> UpdateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel, TResultModel>(
			TModel model,
			Lazy<TRepository> repository,
			CancellationToken cancellationToken,
			Func<TModel, TEntity, TEntity>? mapperInputCallback = null,
			Func<TEntity, Task<IActionResult?>>? beforeUpdateEntityCallback = null,
			Func<TEntity, TResultModel>? mapperOutputCallback = null,
			Func<TEntity, TResultModel, Task>? afterUpdateEntityCallback = null,
			IncludeMap<TEntity>? map = null,
			TRepositoryOptions? options = null,
			bool enableAuthorizationChecks = true,
			bool synchronizeAccess = false,
			bool enableOutputMapping = true)
			where TEntity : class, IEntity<TEntityKey>
			where TEntityKey : IEquatable<TEntityKey>
			where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
			where TRepositoryOptions : RepoOptions, new()
			where TModel : IUpdateModel<TEntityKey>
			where TResultModel : IUpdateResultModel, new()
		{
			cancellationToken.ThrowIfCancellationRequested();

			ISynchronizationRoot? syncRoot = null;

			try
			{
				if (synchronizeAccess && model.Id.ToString() is string syncKey)
					syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

				var entity = await repository.Value.FindByIdAsync(model.Id, cancellationToken, true, map, options).ConfigureAwait(false);

				if (entity is null)
					return NotFound("The specified item could not be found.");

				// Check the concurrency stamp. This is done in the repos and again when the database query is executed but good to fail on
				// this as early as possible.
				if (entity is IConcurrencyStamp concurrencyStamp && concurrencyStamp.ConcurrencyStamp != model.ConcurrencyStamp)
					return ConcurrencyConflict(ConcurrencyErrorMessage);

				entity = mapperInputCallback is null
						? Mapper.Map(model, entity)
						: mapperInputCallback(model, entity);

				if (beforeUpdateEntityCallback != null)
				{
					IActionResult? beforeResult = await beforeUpdateEntityCallback(entity).ConfigureAwait(false);

					if (beforeResult != null)
						return beforeResult;
				}

				// Ensure the current user has Update permissions.
				if (enableAuthorizationChecks)
				{
					AuthorizationResult authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.UpdatePolicyName).ConfigureAwait(false);

					if (!authResult.Succeeded)
						return Forbidden("You do not have the permissions to update the specified item.");
				}

				SaveResult<TEntity> saveResult = await repository.Value.SaveAsync(entity, cancellationToken, repoOptions: options).ConfigureAwait(false);

				if (saveResult.Success)
				{
					TResultModel result = default;

					if (enableOutputMapping)
					{
						result = mapperOutputCallback is null
							? Mapper.Map<TResultModel>(entity)
							: mapperOutputCallback(entity);
					}
					else
					{
						result = new TResultModel();

						if (entity is IConcurrencyStamp concurrencyStampEntity)
							result.ConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
					}

					if (afterUpdateEntityCallback != null)
						await afterUpdateEntityCallback(entity, result).ConfigureAwait(false);

					return Ok(result);
				}

				if (saveResult.ValidationResults?.Count > 0)
					return ValidationProblem(saveResult.ValidationResults.ToModelStateDictionary());

				return BadRequest("There was a problem updating the item. Please try again.");
			}
			catch (UmbrellaDataAccessConcurrencyException)
			{
				return ConcurrencyConflict(ConcurrencyErrorMessage);
			}
			catch (Exception exc) when (Options.UpdateExceptionFilter(exc))
			{
				IActionResult? result = await Options.HandleUpdateExceptionAsync(exc);

				if (result != null)
					return result;

				throw;
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: !IsDevelopment))
			{
				return InternalServerError("There has been a problem updating the specified item.");
			}
			finally
			{
				if (syncRoot != null)
					await syncRoot.DisposeAsync().ConfigureAwait(false);
			}
		}

		protected async Task<IActionResult> DeleteAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions>(
			TEntityKey id,
			Lazy<TRepository> repository,
			CancellationToken cancellationToken,
			Func<TEntity, CancellationToken, Task<IActionResult?>> beforeDeleteEntityAsyncCallback,
			Func<TEntity, CancellationToken, Task> afterDeleteEntityAsyncCallback,
			IncludeMap<TEntity>? map = null,
			TRepositoryOptions? options = null,
			bool enableAuthorizationChecks = true,
			bool synchronizeAccess = false)
			where TEntity : class, IEntity<TEntityKey>
			where TEntityKey : IEquatable<TEntityKey>
			where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
			where TRepositoryOptions : RepoOptions, new()
		{
			cancellationToken.ThrowIfCancellationRequested();

			ISynchronizationRoot? syncRoot = null;

			try
			{
				if (synchronizeAccess && id.ToString() is string syncKey)
					syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

				var entity = await repository.Value.FindByIdAsync(id, cancellationToken, true, map, options).ConfigureAwait(false);

				if (entity is null)
					return NotFound("The specified item could not be found.");

				if (enableAuthorizationChecks)
				{
					var authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.DeletePolicyName).ConfigureAwait(false);

					if (!authResult.Succeeded)
						return Forbidden("The specified item cannot be deleted using your current credentials.");
				}

				IActionResult? result = await beforeDeleteEntityAsyncCallback(entity, cancellationToken).ConfigureAwait(false);

				if (result != null)
					return result;

				await repository.Value.DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
				await afterDeleteEntityAsyncCallback(entity, cancellationToken).ConfigureAwait(false);

				return NoContent();
			}
			catch (Exception exc) when (Options.DeleteExceptionFilter(exc))
			{
				IActionResult? result = await Options.HandleDeleteExceptionAsync(exc);

				if (result != null)
					return result;

				throw;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id }, returnValue: !IsDevelopment))
			{
				return InternalServerError("There has been a problem deleting the specified item.");
			}
			finally
			{
				if (syncRoot != null)
					await syncRoot.DisposeAsync().ConfigureAwait(false);
			}
		}

		protected virtual void ClampPaginationParameters(ref int pageNumber, ref int pageSize)
		{
			pageNumber = Math.Max(pageNumber, 1);
			pageSize = Math.Clamp(pageSize, 1, 50);
		}

		protected virtual (Type type, string key)? GetCreateSynchronizationRootKey<TCreateModel>(TCreateModel model) => null;
	}
}
