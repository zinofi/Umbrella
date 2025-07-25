﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
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

// TODO: Maybe we could allow shaped queries for the ReadAsync method too?
// Also, could look into creating an overload of the Generic controller that allows specifying shaped entities to avoid having to create
// custom endpoints? Would need a generic solution to this though somehow so we don't need to create a new method on the generic repository.
// Hmmmm... Maybe just do the changes for ReadAsync for now to make doing that a bit easier.
// Will need a GetEndpoint property on GenericRemoteRepository and pass this down to the call as per FindAllSlimEndpoint.

// TODO: We could expand on this approach with an UmbrellaDataAccessPageModel and UmbrellaGenericRepositoryPageModel
// Also, an UmbrellaDataAccessController and UmbrellaGenericRepositoryController.
// We could encapsulate all of the core logic into a utility class of some kind and pass in a enum to change the behaviour, e.g. Mode = Api, Mvc, RazorPage
// and that would then determine if we returned, e.g. Page(model) - RazorPages, View(model) - Mvc, Ok(model) - Api.
// Would be a good way of doing things for non-blazor projects where we use server page rendering.

// TODO: For the Create and Update methods, add in a before mapping callback.
// Also, pass in the incoming model to all callbacks where possible.

/// <summary>
/// A generic API Controller that can be used to perform CRUD operations on entities that interact with types that implement <see cref="IGenericDbRepository{TEntity, TRepoOptions, TEntityKey}"/>.
/// </summary>
/// <remarks>
/// This controller is the basis for the <see cref="UmbrellaGenericRepositoryApiController{TSlimModel, TPaginatedResultModel, TModel, TCreateModel, TCreateResultModel, TUpdateModel, TUpdateResultModel, TRepository, TEntity, TRepositoryOptions, TEntityKey}"/>
/// which is easier to use and maintain so consider using that in the first instance instead of this controller.
/// </remarks>
/// <seealso cref="UmbrellaApiController" />
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
	/// Gets the data access unit of work.
	/// </summary>
	protected IDataAccessUnitOfWork DataAccessUnitOfWork { get; } // TODO: Lazy

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessApiController"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hostingEnvironment">The hosting environment.</param>
	/// <param name="options">The options.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="authorizationService">The authorization service.</param>
	/// <param name="synchronizationManager">The synchronization manager.</param>
	/// <param name="dataAccessUnitOfWork">The data access unit of work.</param>
	protected UmbrellaDataAccessApiController(
		ILogger logger,
		IWebHostEnvironment hostingEnvironment,
		UmbrellaDataAccessApiControllerOptions options,
		IUmbrellaMapper mapper,
		IAuthorizationService authorizationService,
		ISynchronizationManager synchronizationManager,
		IDataAccessUnitOfWork dataAccessUnitOfWork) // TODO: Lazy
		: base(logger, hostingEnvironment)
	{
		Options = options;
		Mapper = mapper;
		AuthorizationService = authorizationService;
		SynchronizationManager = synchronizationManager;
		DataAccessUnitOfWork = dataAccessUnitOfWork;
	}

	/// <summary>
	/// Used to load paginated entities in bulk from the repository based on the specified <paramref name="sorters"/> and <paramref name="filters"/>
	/// with each result mapped to a collection of <typeparamref name="TItemModel"/> wrapped in a <typeparamref name="TPaginatedResultModel"/>.
	/// </summary>
	/// <remarks>
	/// The lifecycle of this method internally is as follows:
	/// <list type="number">
	/// <item>Invokes the <paramref name="loadReadAllDataAsyncDelegate"/> to load all <typeparamref name="TEntityResult"/> instances from the repository.</item>
	/// <item>Perform authorization, if enabled via the <paramref name="enableAuthorizationChecks"/> property, on all loaded entities.</item>
	/// <item>Creates the <typeparamref name="TPaginatedResultModel"/> and maps the <typeparamref name="TEntityResult"/> instances to <typeparamref name="TItemModel"/> instances using the the <paramref name="mapReadAllEntitiesDelegate"/>, falling back to using the <see cref="Mapper"/> if not specified.</item>
	/// <item>Invokes the <paramref name="afterCreateSearchSlimPaginatedModelAsyncDelegate"/> to allow the <typeparamref name="TPaginatedResultModel"/> to be augmented.</item>
	/// <item>Invokes the <paramref name="afterCreateSlimModelAsyncDelegate"/> on each instance of <typeparamref name="TItemModel"/> on the <typeparamref name="TPaginatedResultModel"/> to allow each instance to be augmented.</item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TEntityResult">The type of the entity result.</typeparam>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <param name="filterCombinator">The filter combinator.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="loadReadAllDataAsyncDelegate">The delegate used to load the data.</param>
	/// <param name="mapReadAllEntitiesDelegate">The delegate used to map the results loaded from the repository. If not specified, the <see cref="Mapper"/> is used.</param>
	/// <param name="afterCreateSearchSlimPaginatedModelAsyncDelegate">The delegate that is invoked after the paginated result model has been created.</param>
	/// <param name="afterCreateSlimModelAsyncDelegate">The delegate that is invoked when result models have been created.</param>
	/// <param name="options">The repository options.</param>
	/// <param name="childOptions">The child repository options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities loaded from the repository.</param>
	/// <returns>
	/// The action result containing the endpoint response which either be a <typeparamref name="TPaginatedResultModel"/> when successful or
	/// a <see cref="ProblemDetails"/> response and / or erroneous state code as appropriate.
	/// </returns>
	protected virtual async Task<IActionResult> ReadAllAsync<TEntityResult, TEntity, TEntityKey, TRepositoryOptions, TItemModel, TPaginatedResultModel>(
		int pageNumber,
		int pageSize,
		SortExpression<TEntityResult>[]? sorters,
		FilterExpression<TEntity>[]? filters,
		FilterExpressionCombinator? filterCombinator,
		Func<int, int, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, TRepositoryOptions?, IEnumerable<RepoOptions>?, CancellationToken, Task<PaginatedResultModel<TEntityResult>>> loadReadAllDataAsyncDelegate,
		CancellationToken cancellationToken,
		Func<IReadOnlyCollection<TEntityResult>, TItemModel[]>? mapReadAllEntitiesDelegate = null,
		Func<PaginatedResultModel<TEntityResult>, TPaginatedResultModel, SortExpression<TEntityResult>[]?, FilterExpression<TEntity>[]?, FilterExpressionCombinator?, CancellationToken, Task>? afterCreateSearchSlimPaginatedModelAsyncDelegate = null,
		Func<TEntityResult, TItemModel, CancellationToken, Task<IActionResult?>>? afterCreateSlimModelAsyncDelegate = null,
		TRepositoryOptions? options = null,
		IEnumerable<RepoOptions>? childOptions = null,
		bool enableAuthorizationChecks = true)
		where TEntityResult : class, IEntity<TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TEntityKey : IEquatable<TEntityKey>
		where TRepositoryOptions : RepoOptions, new()
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>, new()
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(loadReadAllDataAsyncDelegate);

		try
		{
			ClampPaginationParameters(ref pageNumber, ref pageSize);

			PaginatedResultModel<TEntityResult> result = await loadReadAllDataAsyncDelegate(pageNumber, pageSize, sorters, filters, filterCombinator, options, childOptions, cancellationToken).ConfigureAwait(false);

			// Authorization Checks
			if (enableAuthorizationChecks)
			{
				bool authorized = await AuthorizationService.AuthorizeAllAsync(User, result.Items, Options.ReadPolicyName, cancellationToken).ConfigureAwait(false);

				if (!authorized)
					return Forbidden("There are items that are forbidden from being accessed in the results.");
			}

			var model = new TPaginatedResultModel
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
			IActionResult? result = await Options.HandleReadAllExceptionAsync(exc).ConfigureAwait(false);

			if (result is not null)
				return result;

			throw;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { pageNumber, pageSize, sorters = sorters?.ToSortExpressionDescriptors(), filters = filters?.ToFilterExpressionDescriptors() }, returnValue: !IsDevelopment))
		{
			return InternalServerError("An error has occurred whilst trying to get the list of items.");
		}
	}

	/// <summary>
	/// Used to load a single <typeparamref name="TEntity"/> in from the repository based on the specified <paramref name="id"/> and return a
	/// mapped <typeparamref name="TModel"/>. 
	/// </summary>
	/// <remarks>
	/// The lifecycle of this method internally is as follows:
	/// <list type="number">
	/// <item>Synchronize execution of this method if enabled using <paramref name="synchronizeAccess"/>.</item>
	/// <item>Load the <typeparamref name="TEntity"/> from the <typeparamref name="TRepository"/>.</item>
	/// <item>Perform authorization, if enabled via the <paramref name="enableAuthorizationChecks"/> property, on the entity.</item>
	/// <item>Maps the <typeparamref name="TEntity"/> to the <typeparamref name="TModel"/> using the <paramref name="mapperCallback"/> falling back to use the <see cref="Mapper"/> if not specified.</item>
	/// <item>Invokes the <paramref name="afterReadEntityCallback"/> to augment the <typeparamref name="TModel"/>.</item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	/// <param name="id">The identifier.</param>
	/// <param name="repository">The repository.</param>
	/// <param name="loadReadEntityAsyncDelegate">The optional delegate used to load the entity. If not specified, the <c>FindByIdAsync</c> method on the <paramref name="repository"/> will be used.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="mapperCallback">The mapper callback. If not specified, <see cref="Mapper"/> is used to perform the mapping.</param>
	/// <param name="afterReadEntityCallback">The after read entity callback.</param>
	/// <param name="trackChanges">if set to <see langword="true"/> enable change tracking on the database context.</param>
	/// <param name="map">The map.</param>
	/// <param name="options">The options.</param>
	/// <param name="childOptions">The child options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities loaded from the repository.</param>
	/// <param name="synchronizeAccess">Specifies whether exclusive access should be enabled using code that synchronizes using the <paramref name="id"/> and type name of the entity.</param>
	/// <returns>
	/// The action result containing the endpoint response which either be a <typeparamref name="TModel"/> when successful or
	/// a <see cref="ProblemDetails"/> response and / or erroneous state code as appropriate.
	/// </returns>
	protected async Task<IActionResult> ReadAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel>(
		TEntityKey id,
		Lazy<TRepository> repository,
		CancellationToken cancellationToken,
		Func<TEntityKey, bool, IncludeMap<TEntity>?, TRepositoryOptions?, IEnumerable<RepoOptions>?, CancellationToken, Task<TEntity?>>? loadReadEntityAsyncDelegate = null,
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
		Guard.IsNotNull(repository);

		ISynchronizationRoot? syncRoot = null;

		try
		{
			if (synchronizeAccess && id.ToString() is string syncKey)
				syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

			TEntity? item = loadReadEntityAsyncDelegate is not null
				? await loadReadEntityAsyncDelegate(id, trackChanges, map, options, childOptions, cancellationToken).ConfigureAwait(false)
				: await repository.Value.FindByIdAsync(id, trackChanges, map, options, childOptions, cancellationToken).ConfigureAwait(false);

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
			IActionResult? result = await Options.HandleReadExceptionAsync(exc).ConfigureAwait(false);

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

	/// <summary>
	/// Used to create a new <typeparamref name="TEntity"/> in the repository based on the provided <typeparamref name="TModel"/> which returns
	/// a <typeparamref name="TResultModel"/> if successful.
	/// </summary>
	/// <remarks>
	/// The lifecycle of this method internally is as follows:
	/// <list type="number">
	/// <item>Synchronize execution of this method if enabled using <paramref name="synchronizeAccess"/>.</item>
	/// <item>Maps the <typeparamref name="TModel"/> to a new instance of <typeparamref name="TEntity"/> using the <paramref name="mapperInputCallback"/> falling back to use the <see cref="Mapper"/> if not specified.</item>
	/// <item>Invokes the <paramref name="beforeCreateEntityCallback"/> to augment the newly created <typeparamref name="TEntity"/>.</item>
	/// <item>Perform authorization, if enabled via the <paramref name="enableAuthorizationChecks"/> property, on the entity.</item>
	/// <item>Saves the new entity to the <typeparamref name="TRepository"/>.</item>
	/// <item>
	/// Creates the <typeparamref name="TResultModel"/> by mapping the entity, if <paramref name="enableOutputMapping"/> is <see langword="true"/>, using the <paramref name="mapperOutputCallback"/> if specified falling back to using the <see cref="Mapper"/>.
	/// If <paramref name="enableOutputMapping"/> is <see langword="true"/>, a new instance of <typeparamref name="TResultModel"/> and only assigning the <c>Id</c> and <c>ConcurrencyStamp</c> properties.
	/// </item>
	/// <item>
	/// Invokes the <paramref name="afterCreateEntityCallback"/> that can be specified to augment the <typeparamref name="TResultModel"/>.
	/// </item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	/// <typeparam name="TResultModel">The type of the result model.</typeparam>
	/// <param name="model">The model.</param>
	/// <param name="repository">The repository.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="mapperInputCallback">The mapper input callback.</param>
	/// <param name="beforeCreateEntityCallback">The before create entity callback.</param>
	/// <param name="mapperOutputCallback">The mapper output callback.</param>
	/// <param name="afterCreateEntityCallback">The after create entity callback.</param>
	/// <param name="options">The options.</param>
	/// <param name="childOptions">The child options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities persisted to the repository.</param>
	/// <param name="synchronizeAccess">Specifies whether exclusive access should be enabled using code that synchronizes using a key generated using the <see cref="GetCreateSynchronizationRootKey"/> method. This method must be overridden on the controller to make this work.</param>
	/// <param name="enableOutputMapping">
	/// Specifies whether the newly created <typeparamref name="TEntity"/> is mapped to an instance of <typeparamref name="TResultModel"/> using the <see cref="Mapper"/>,
	/// or if this is done by this method internally by creating a new instance of <typeparamref name="TResultModel"/> and only assigning the <c>Id</c> and <c>ConcurrencyStamp</c> properties.
	/// Please leave this set to <see langword="true"/> use a mapping implementation for a richer experience.
	/// </param>
	/// <returns>
	/// The action result containing the endpoint response which either be a <typeparamref name="TResultModel"/> when successful or
	/// a <see cref="ProblemDetails"/> response and / or erroneous state code as appropriate.
	/// </returns>
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
		Guard.IsNotNull(repository);

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

			OperationResult<TEntity> saveResult = await repository.Value.SaveEntityAsync(entity, repoOptions: options, childOptions: childOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
			await DataAccessUnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

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
			IActionResult? result = await Options.HandleCreateExceptionAsync(exc).ConfigureAwait(false);

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

	/// <summary>
	/// Used to update an existing <typeparamref name="TEntity"/> in the repository based on the provided <typeparamref name="TModel"/> which returns
	/// a <typeparamref name="TResultModel"/> if successful.
	/// </summary>
	/// <remarks>
	/// The lifecycle of this method internally is as follows:
	/// <list type="number">
	/// <item>Synchronize execution of this method if enabled using <paramref name="synchronizeAccess"/>.</item>
	/// <item>Loads the existing <typeparamref name="TEntity"/> from the <typeparamref name="TRepository"/>.</item>
	/// <item>Checks for mismatches between <c>ConcurrencyStamp</c> property values.</item>
	/// <item>Invokes the <paramref name="beforeMappingCallback"/> to allow execution of custom code before performing mapping.</item>
	/// <item>Maps the <typeparamref name="TModel"/> to an existing instance of <typeparamref name="TEntity"/> using the <paramref name="mapperInputCallback"/> falling back to use the <see cref="Mapper"/> if not specified.</item>
	/// <item>Invokes the <paramref name="beforeUpdateEntityCallback"/> to augment the updated <typeparamref name="TEntity"/>.</item>
	/// <item>Perform authorization, if enabled via the <paramref name="enableAuthorizationChecks"/> property, on the entity.</item>
	/// <item>Saves the updated entity to the <typeparamref name="TRepository"/>.</item>
	/// <item>
	/// Creates the <typeparamref name="TResultModel"/> by mapping the entity, if <paramref name="enableOutputMapping"/> is <see langword="true"/>, using the <paramref name="mapperOutputCallback"/> if specified falling back to using the <see cref="Mapper"/>.
	/// If <paramref name="enableOutputMapping"/> is <see langword="true"/>, a new instance of <typeparamref name="TResultModel"/> and only assigning the <c>Id</c> and <c>ConcurrencyStamp</c> properties.
	/// </item>
	/// <item>
	/// Invokes the <paramref name="afterUpdateEntityCallback"/> that can be specified to augment the <typeparamref name="TResultModel"/>.
	/// </item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	/// <typeparam name="TResultModel">The type of the result model.</typeparam>
	/// <param name="model">The model.</param>
	/// <param name="repository">The repository.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="beforeMappingCallback">The optional callback invoked before mapping.</param>
	/// <param name="mapperInputCallback">The mapper input callback.</param>
	/// <param name="beforeUpdateEntityCallback">The before update entity callback.</param>
	/// <param name="mapperOutputCallback">The mapper output callback.</param>
	/// <param name="afterUpdateEntityCallback">The after update entity callback.</param>
	/// <param name="map">The map.</param>
	/// <param name="options">The options.</param>
	/// <param name="childOptions">The child options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities persisted to the repository.</param>
	/// <param name="synchronizeAccess">Specifies whether exclusive access should be enabled using code that synchronizes using the <c>Id</c> and type name of the entity.</param>
	/// <param name="enableOutputMapping">
	/// Specifies whether the newly created <typeparamref name="TEntity"/> is mapped to an instance of <typeparamref name="TResultModel"/> using the <see cref="Mapper"/>,
	/// or if this is done by this method by creating a new instance of <typeparamref name="TResultModel"/> and only assigning the <c>ConcurrencyStamp</c> property.
	/// Please leave this set to <see langword="true"/> use a mapping implementation for a richer experience.
	/// </param>
	/// <returns>
	/// The action result containing the endpoint response which either be a <typeparamref name="TResultModel"/> when successful or
	/// a <see cref="ProblemDetails"/> response and / or erroneous state code as appropriate.
	/// </returns>
	protected async Task<IActionResult> UpdateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel, TResultModel>(
		TModel model,
		Lazy<TRepository> repository,
		CancellationToken cancellationToken,
		Func<TEntity, Task<IActionResult?>>? beforeMappingCallback = null,
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
		Guard.IsNotNull(repository);

		try
		{
			if (synchronizeAccess && model.Id.ToString() is string syncKey)
				syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

			var entity = await repository.Value.FindByIdAsync(model.Id, true, map, options, cancellationToken: cancellationToken).ConfigureAwait(false);

			if (entity is null)
				return NotFound("The specified item could not be found.");

			// Check the concurrency stamp. This is done in the repos and again when the database query is executed but good to fail on
			// this as early as possible.
			if (entity is IConcurrencyStamp concurrencyStamp && concurrencyStamp.ConcurrencyStamp != model.ConcurrencyStamp)
				return ConcurrencyConflict(Options.ConcurrencyErrorMessage);

			if (beforeMappingCallback is not null)
			{
				var result = await beforeMappingCallback(entity).ConfigureAwait(false);

				if (result is not null)
					return result;
			}

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

			OperationResult<TEntity> saveResult = await repository.Value.SaveEntityAsync(entity, repoOptions: options, childOptions: childOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
			await DataAccessUnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

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
			IActionResult? result = await Options.HandleUpdateExceptionAsync(exc).ConfigureAwait(false);

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

	/// <summary>
	/// Used to delete an existing <typeparamref name="TEntity"/> in the repository based on the provided <paramref name="id"/> which returns
	/// a <c>204 - No Content</c> status code if successful.
	/// </summary>
	/// <remarks>
	/// The lifecycle of this method internally is as follows:
	/// <list type="number">
	/// <item>Synchronize execution of this method if enabled using <paramref name="synchronizeAccess"/>.</item>
	/// <item>Loads the existing <typeparamref name="TEntity"/> from the <typeparamref name="TRepository"/>.</item>
	/// <item>Perform authorization, if enabled via the <paramref name="enableAuthorizationChecks"/> property, on the entity.</item>
	/// <item>Invokes the <paramref name="beforeDeleteEntityAsyncCallback"/> to perform work before deleting the entity.</item>
	/// <item>Deletes the entity from the repository.</item>
	/// <item>Invokes the <paramref name="afterDeleteEntityAsyncCallback"/> to perform work after the entity has been deleted.</item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
	/// <param name="id">The identifier.</param>
	/// <param name="repository">The repository.</param>
	/// <param name="beforeDeleteEntityAsyncCallback">The before delete entity asynchronous callback.</param>
	/// <param name="afterDeleteEntityAsyncCallback">The after delete entity asynchronous callback.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="map">The map.</param>
	/// <param name="options">The options.</param>
	/// <param name="childOptions">The child options.</param>
	/// <param name="enableAuthorizationChecks">Specifies whether imperative authorization checks are performed on entities persisted to the repository.</param>
	/// <param name="synchronizeAccess">Specifies whether exclusive access should be enabled using code that synchronizes using the <paramref name="id"/> and type name of the entity.</param>
	/// <returns>
	/// The action result containing the endpoint response which either be a <c>204 - No Content</c> status code when successful or
	/// a <see cref="ProblemDetails"/> response and / or erroneous state code as appropriate.
	/// </returns>
	protected async Task<IActionResult> DeleteAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions>(
		TEntityKey id,
		Lazy<TRepository> repository,
		Func<TEntity, CancellationToken, Task<IActionResult?>> beforeDeleteEntityAsyncCallback,
		Func<TEntity, CancellationToken, Task> afterDeleteEntityAsyncCallback,
		CancellationToken cancellationToken,
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
		Guard.IsNotNull(repository);
		Guard.IsNotNull(beforeDeleteEntityAsyncCallback);
		Guard.IsNotNull(afterDeleteEntityAsyncCallback);

		ISynchronizationRoot? syncRoot = null;

		try
		{
			if (synchronizeAccess && id.ToString() is string syncKey)
				syncRoot = await SynchronizationManager.GetSynchronizationRootAndWaitAsync<TEntity>(syncKey, cancellationToken).ConfigureAwait(false);

			var entity = await repository.Value.FindByIdAsync(id, true, map, options, childOptions, cancellationToken).ConfigureAwait(false);

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

			await repository.Value.DeleteEntityAsync(entity, repoOptions: options, childOptions: childOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
			await DataAccessUnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
			await afterDeleteEntityAsyncCallback(entity, cancellationToken).ConfigureAwait(false);

			return NoContent();
		}
		catch (Exception exc) when (Options.DeleteExceptionFilter(exc))
		{
			IActionResult? result = await Options.HandleDeleteExceptionAsync(exc).ConfigureAwait(false);

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

// Design
// We need an UmbrellaDataAccessService
// This service should have the same async methods as the controller
// i.e. ReadAsync, ReadAllAsync, CreateAsync, UpdateAsync, DeleteAsync
// We should inject this service into the controller and use it to perform the operations
// Can be abstract away the authorization service and use a custom abstraction for authorization checks?
// The public methods should be simple. Create internal overloads that do a bit more work and take in the repository, options, etc.

// The return types of the public methods should be OperationResult. Change OperationResult to be a record?
// We need to extend the OperationResult to include any additional information we want to return

// We need an IUmbrellaDataAccessService interface that defines the methods
// We then need an implementation that use Http endpoints instead of repositories

public abstract class UmbrellaDataAccessService
{

}