// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Shared.Models;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Umbrella.AspNetCore.WebUtilities.Mvc.Options;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Data.Concurrency;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Mapping.Abstractions;
using Umbrella.Utilities.Primitives;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

// TODO: We could expand on this approach with an UmbrellaDataAccessPageModel and UmbrellaGenericRepositoryPageModel
// Also, an UmbrellaDataAccessController and UmbrellaGenericRepositoryController.
// We could encapsulate all of the core logic into a utility class of some kind and pass in a enum to change the behaviour, e.g. Mode = Api, Mvc, RazorPage
// and that would then determine if we returned, e.g. Page(model) - RazorPages, View(model) - Mvc, Ok(model) - Api.
// Would be a good way of doing things for non-blazor projects where we use server page rendering.
public abstract class UmbrellaDataAccessApiController : UmbrellaApiController
{
	/// <summary>
	/// Gets the options.
	/// </summary>
	protected UmbrellaDataAccessApiControllerOptions Options { get; }

	/// <summary>
	/// Gets the mapper.
	/// </summary>
	protected IUmbrellaMapper Mapper { get; }

	/// <summary>
	/// Gets the authorization service.
	/// </summary>
	protected IAuthorizationService AuthorizationService { get; }

	/// <summary>
	/// Gets the synchronization manager.
	/// </summary>
	protected ISynchronizationManager SynchronizationManager { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessApiController"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hostingEnvironment">The hosting environment.</param>
	/// <param name="options">The options.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="authorizationService">The authorization service.</param>
	/// <param name="synchronizationManager">The synchronization manager.</param>
	protected UmbrellaDataAccessApiController(
		ILogger logger,
		IWebHostEnvironment hostingEnvironment,
		UmbrellaDataAccessApiControllerOptions options,
		IUmbrellaMapper mapper,
		IAuthorizationService authorizationService,
		ISynchronizationManager synchronizationManager)
		: base(logger, hostingEnvironment)
	{
		Options = options;
		Mapper = mapper;
		AuthorizationService = authorizationService;
		SynchronizationManager = synchronizationManager;
	}

	/// <summary>
	/// Reads all items from a repository.
	/// </summary>
	/// <typeparam name="TEntityResult">The type of the entity result.</typeparam>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TPaginatedItemModel">The type of the paginated item model.</typeparam>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <param name="filterCombinator">The filter combinator.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="loadReadAllDataAsyncDelegate">The delegate used to load the data.</param>
	/// <param name="mapReadAllEntitiesDelegate">The delegate used to map the results loaded from the repository.</param>
	/// <param name="afterCreateSearchSlimPaginatedModelAsyncDelegate">The delegate that is invoked after the paginated result model has been created.</param>
	/// <param name="afterCreateSlimModelAsyncDelegate">The delegate that is invoked when result models have been created.</param>
	/// <param name="options">The repository options.</param>
	/// <param name="childOptions">The child repository options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities loaded from the repository.</param>
	/// <returns>An action result with the result of the operation.</returns>
	protected virtual async Task<IActionResult> ReadAllAsync<TEntityResult, TEntity, TEntityKey, TRepositoryOptions, TItemModel, TPaginatedItemModel>(
		int pageNumber,
		int pageSize,
		SortExpression<TEntityResult>[]? sorters,
		FilterExpression<TEntity>[]? filters,
		FilterExpressionCombinator? filterCombinator,
		CancellationToken cancellationToken,
		Func<int, int, CancellationToken, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, TRepositoryOptions?, IEnumerable<RepoOptions>?, Task<PaginatedResultModel<TEntityResult>>> loadReadAllDataAsyncDelegate,
		Func<IReadOnlyCollection<TEntityResult>, TItemModel[]>? mapReadAllEntitiesDelegate = null,
		Func<PaginatedResultModel<TEntityResult>, TPaginatedItemModel, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, CancellationToken, Task>? afterCreateSearchSlimPaginatedModelAsyncDelegate = null,
		Func<TEntityResult, TItemModel, CancellationToken, Task<IActionResult?>>? afterCreateSlimModelAsyncDelegate = null,
		TRepositoryOptions? options = null,
		IEnumerable<RepoOptions>? childOptions = null,
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

			PaginatedResultModel<TEntityResult> result = await loadReadAllDataAsyncDelegate(pageNumber, pageSize, cancellationToken, sorters, filters, filterCombinator, options, childOptions).ConfigureAwait(false);

			// Authorization Checks
			if (enableAuthorizationChecks)
			{
				bool authorized = await AuthorizationService.AuthorizeAllAsync(User, result.Items, Options.ReadPolicyName, cancellationToken).ConfigureAwait(false);

				if (!authorized)
					return Forbidden("There are items that are forbidden from being accessed in the results.");
			}

			var model = new TPaginatedItemModel
			{
				Items = mapReadAllEntitiesDelegate is null ? await Mapper.MapAllAsync<TItemModel>(result.Items, cancellationToken).ConfigureAwait(false) : mapReadAllEntitiesDelegate(result.Items),
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = result.TotalCount,
				MoreItems = pageNumber * pageSize < result.TotalCount
			};

			if (afterCreateSearchSlimPaginatedModelAsyncDelegate is not null)
				await afterCreateSearchSlimPaginatedModelAsyncDelegate(result, model, sorters, filters, filterCombinator, cancellationToken).ConfigureAwait(false);

			if (afterCreateSlimModelAsyncDelegate is not null)
			{
				for (int i = 0; i < model.Items.Count; i++)
				{
					IActionResult? actionResult = await afterCreateSlimModelAsyncDelegate(result.Items.ElementAt(i), model.Items.ElementAt(i), cancellationToken).ConfigureAwait(false);

					if (actionResult is not null)
						return actionResult;
				}
			}

			return Ok(model);
		}
		catch (Exception exc) when (Options.ReadAllExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleReadAllExceptionAsync(exc);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { pageNumber, pageSize, sorters = sorters?.ToSortExpressionDescriptors(), filters = filters?.ToFilterExpressionDescriptors() }, returnValue: !IsDevelopment))
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
		IEnumerable<RepoOptions>? childOptions = null,
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

			TEntity? item = await repository.Value.FindByIdAsync(id, cancellationToken, trackChanges, map, options, childOptions).ConfigureAwait(false);

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
				? await Mapper.MapAsync<TModel>(item, cancellationToken).ConfigureAwait(false)
				: mapperCallback(item);

			if (afterReadEntityCallback is not null)
			{
				IActionResult? result = await afterReadEntityCallback(item, model).ConfigureAwait(false);

				if (result is not null)
					return result;
			}

			return Ok(model);
		}
		catch (Exception exc) when (Options.ReadExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleReadExceptionAsync(exc);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { id }, returnValue: !IsDevelopment))
		{
			return InternalServerError("There has been a problem getting the specified item.");
		}
		finally
		{
			if (syncRoot is not null)
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
		IEnumerable<RepoOptions>? childOptions = null,
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
			if (model is null)
				return BadRequest("The request body has not been provided.");

			if (synchronizeAccess)
			{
				(Type type, string key)? syncKey = GetCreateSynchronizationRootKey(model!);

				if (syncKey.HasValue)
					syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync(syncKey.Value.type, syncKey.Value.key, cancellationToken).ConfigureAwait(false);
			}

			var entity = mapperInputCallback is null
				? await Mapper.MapAsync<TEntity>(model, cancellationToken).ConfigureAwait(false)
				: mapperInputCallback(model);

			if (beforeCreateEntityCallback is not null)
			{
				IActionResult? beforeResult = await beforeCreateEntityCallback(entity).ConfigureAwait(false);

				if (beforeResult is not null)
					return beforeResult;
			}

			// Ensure the current user has Create permissions.
			if (enableAuthorizationChecks)
			{
				AuthorizationResult authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.CreatePolicyName).ConfigureAwait(false);

				if (!authResult.Succeeded)
					return Forbidden("You do not have permission to access the specified item.");
			}

			OperationResult<TEntity> saveResult = await repository.Value.SaveAsync(entity, cancellationToken, repoOptions: options, childOptions: childOptions).ConfigureAwait(false);

			if (saveResult.Status is OperationResultStatus.Success)
			{
				TResultModel? result = default;

				if (enableOutputMapping)
				{
					result = mapperOutputCallback is null
						? await Mapper.MapAsync<TResultModel>(entity, cancellationToken).ConfigureAwait(false)
						: mapperOutputCallback(entity);
				}
				else
				{
					result = new TResultModel { Id = entity.Id };

					if (result is IConcurrencyStamp concurrencyStampResult && entity is IConcurrencyStamp concurrencyStampEntity)
						concurrencyStampResult.ConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
				}

				if (afterCreateEntityCallback is not null)
					await afterCreateEntityCallback(entity, result).ConfigureAwait(false);

				return Created(result);
			}

			return saveResult.ValidationResults?.Count > 0
				? ValidationProblem(saveResult.ValidationResults.ToModelStateDictionary())
				: (IActionResult)BadRequest("There was a problem saving the item. Please try again.");
		}
		catch (Exception exc) when (Options.CreateExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleCreateExceptionAsync(exc);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, returnValue: !IsDevelopment))
		{
			return InternalServerError("There has been a problem creating the specified item.");
		}
		finally
		{
			if (syncRoot is not null)
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
		IEnumerable<RepoOptions>? childOptions = null,
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
				return ConcurrencyConflict(Options.ConcurrencyErrorMessage);

			entity = mapperInputCallback is null
					? await Mapper.MapAsync(model, entity, cancellationToken).ConfigureAwait(false)
					: mapperInputCallback(model, entity);

			if (beforeUpdateEntityCallback is not null)
			{
				IActionResult? beforeResult = await beforeUpdateEntityCallback(entity).ConfigureAwait(false);

				if (beforeResult is not null)
					return beforeResult;
			}

			// Ensure the current user has Update permissions.
			if (enableAuthorizationChecks)
			{
				AuthorizationResult authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.UpdatePolicyName).ConfigureAwait(false);

				if (!authResult.Succeeded)
					return Forbidden("You do not have the permissions to update the specified item.");
			}

			OperationResult<TEntity> saveResult = await repository.Value.SaveAsync(entity, cancellationToken, repoOptions: options, childOptions: childOptions).ConfigureAwait(false);

			if (saveResult.Status is OperationResultStatus.Success)
			{
				TResultModel? result = default;

				if (enableOutputMapping)
				{
					result = mapperOutputCallback is null
						? await Mapper.MapAsync<TResultModel>(entity, cancellationToken).ConfigureAwait(false)
						: mapperOutputCallback(entity);
				}
				else
				{
					result = new TResultModel();

					if (entity is IConcurrencyStamp concurrencyStampEntity)
						result.ConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
				}

				if (afterUpdateEntityCallback is not null)
					await afterUpdateEntityCallback(entity, result).ConfigureAwait(false);

				return Ok(result);
			}

			return saveResult.ValidationResults?.Count > 0
				? ValidationProblem(saveResult.ValidationResults.ToModelStateDictionary())
				: (IActionResult)BadRequest("There was a problem updating the item. Please try again.");
		}
		catch (UmbrellaConcurrencyException)
		{
			return ConcurrencyConflict(Options.ConcurrencyErrorMessage);
		}
		catch (Exception exc) when (Options.UpdateExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleUpdateExceptionAsync(exc);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, returnValue: !IsDevelopment))
		{
			return InternalServerError("There has been a problem updating the specified item.");
		}
		finally
		{
			if (syncRoot is not null)
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
		IEnumerable<RepoOptions>? childOptions = null,
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

			var entity = await repository.Value.FindByIdAsync(id, cancellationToken, true, map, options, childOptions).ConfigureAwait(false);

			if (entity is null)
				return NotFound("The specified item could not be found.");

			if (enableAuthorizationChecks)
			{
				var authResult = await AuthorizationService.AuthorizeAsync(User, entity, Options.DeletePolicyName).ConfigureAwait(false);

				if (!authResult.Succeeded)
					return Forbidden("The specified item cannot be deleted using your current credentials.");
			}

			IActionResult? result = await beforeDeleteEntityAsyncCallback(entity, cancellationToken).ConfigureAwait(false);

			if (result is not null)
				return result;

			await repository.Value.DeleteAsync(entity, cancellationToken, repoOptions: options, childOptions: childOptions).ConfigureAwait(false);
			await afterDeleteEntityAsyncCallback(entity, cancellationToken).ConfigureAwait(false);

			return NoContent();
		}
		catch (Exception exc) when (Options.DeleteExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleDeleteExceptionAsync(exc);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { id }, returnValue: !IsDevelopment))
		{
			return InternalServerError("There has been a problem deleting the specified item.");
		}
		finally
		{
			if (syncRoot is not null)
				await syncRoot.DisposeAsync().ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Clamps the pagination parameters.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <remarks>
	/// By default, this clamps the page number to ensure it has a minimum value of <c>1</c>
	/// and the page size to ensure it is between <c>1</c> and <c>50</c> inclusive.
	/// </remarks>
	protected virtual void ClampPaginationParameters(ref int pageNumber, ref int pageSize)
	{
		pageNumber = Math.Max(pageNumber, 1);
		pageSize = Math.Clamp(pageSize, 1, 50);
	}

	/// <summary>
	/// Gets the synchronization key to be used internally by the <see cref="CreateAsync"/> method. It is important
	/// that this key is scoped appropriately to ensure correct locking behaviour.
	/// </summary>
	/// <param name="model">The incoming model passed into the action method.</param>
	/// <returns>A tuple containing the type and the key used to performing synchronization.</returns>
	protected virtual (Type type, string key)? GetCreateSynchronizationRootKey(object model) => null;
}