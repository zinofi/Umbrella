// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Shared.Models;
using Umbrella.AspNetCore.WebUtilities.Mvc.Options;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Mapping.Abstractions;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

/// <summary>
/// A generic API Controller that can be used to perform CRUD operations on entities that interact with types that implement <see cref="IGenericDbRepository{TEntity, TRepoOptions, TEntityKey}"/>.
/// </summary>
/// <typeparam name="TSlimModel">The type of the slim model.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TCreateModel">The type of the create model.</typeparam>
/// <typeparam name="TCreateResultModel">The type of the create result model.</typeparam>
/// <typeparam name="TUpdateModel">The type of the update model.</typeparam>
/// <typeparam name="TUpdateResultModel">The type of the update result model.</typeparam>
/// <typeparam name="TRepository">The type of the repository.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TRepositoryOptions">The type of the repository options.</typeparam>
/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
/// <seealso cref="UmbrellaDataAccessApiController" />
public abstract class UmbrellaGenericRepositoryApiController<TSlimModel, TPaginatedResultModel, TModel, TCreateModel, TCreateResultModel, TUpdateModel, TUpdateResultModel, TRepository, TEntity, TRepositoryOptions, TEntityKey> : UmbrellaDataAccessApiController
	where TPaginatedResultModel : PaginatedResultModel<TSlimModel>, new()
	where TCreateResultModel : ICreateResultModel<TEntityKey>, new()
	where TUpdateModel : IUpdateModel<TEntityKey>
	where TUpdateResultModel : IUpdateResultModel, new()
	where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
	where TEntity : class, IEntity<TEntityKey>
	where TRepositoryOptions : RepoOptions, new()
	where TEntityKey : IEquatable<TEntityKey>
{
	/// <summary>
	/// Gets the repository.
	/// </summary>
	protected Lazy<TRepository> Repository { get; }

	/// <summary>
	/// Gets a value indicating whether the <see cref="SearchSlim"/> endpoint is enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers that should not allow the endpoint to be accessed if it is not needed.
	/// </remarks>
	protected virtual bool SlimReadEndpointEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether the <see cref="Get"/> endpoint is enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers that should not allow the endpoint to be accessed if it is not needed.
	/// </remarks>
	protected virtual bool ReadEndpointEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether the <see cref="Post"/> endpoint is enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers that should not allow the endpoint to be accessed if it is not needed.
	/// </remarks>
	protected virtual bool CreateEndpointEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether the <see cref="Put"/> endpoint is enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers that should not allow the endpoint to be accessed if it is not needed.
	/// </remarks>
	protected virtual bool UpdateEndpointEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether the <see cref="Delete"/> endpoint is enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers that should not allow the endpoint to be accessed if it is not needed.
	/// </remarks>
	protected virtual bool DeleteEndpointEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether imperative authorization checks are performed when the <see cref="SearchSlim"/> endpoint is executed.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers where imperative authorization checks are not required.
	/// </remarks>
	protected virtual bool AuthorizationSlimReadChecksEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether imperative authorization checks are performed when the <see cref="Get"/> endpoint is executed.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers where imperative authorization checks are not required.
	/// </remarks>
	protected virtual bool AuthorizationReadChecksEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether imperative authorization checks are performed when the <see cref="Post"/> endpoint is executed.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers where imperative authorization checks are not required.
	/// </remarks>
	protected virtual bool AuthorizationCreateChecksEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether imperative authorization checks are performed when the <see cref="Put"/> endpoint is executed.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers where imperative authorization checks are not required.
	/// </remarks>
	protected virtual bool AuthorizationUpdateChecksEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether imperative authorization checks are performed when the <see cref="Delete"/> endpoint is executed.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="true" /> but should be overridden and set to <see langword="false" /> in derived
	/// controllers where imperative authorization checks are not required.
	/// </remarks>
	protected virtual bool AuthorizationDeleteChecksEnabled { get; } = true;

	/// <summary>
	/// Gets a value indicating whether or not entities loaded during calls to the <see cref="Get"/> endpoint
	/// are loaded with change tracking enabled.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="false" /> and should only be enabled under exceptional circumstances, e.g. when dealing with EF Core shadow properties.
	/// </remarks>
	protected virtual bool GetTrackChanges { get; }

	/// <summary>
	/// Gets a value indicating whether calls to the <see cref="Get(TEntityKey, CancellationToken)"/> endpoint should be synchronized
	/// using the <see cref="IEntity{TEntityKey}.Id"/> of the entity being loaded.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="false"/>.
	/// </remarks>
	protected virtual bool GetLock { get; }

	/// <summary>
	/// Gets a value indicating whether calls to the <see cref="Post(TCreateModel, CancellationToken)"/> endpoint should be synchronized
	/// using a synchronization key created by a call to the <see cref="UmbrellaDataAccessApiController.GetCreateSynchronizationRootKey(object)"/> method.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="false"/>.
	/// </remarks>
	protected virtual bool PostLock { get; }

	/// <summary>
	/// Gets a value indicating whether calls to the <see cref="Put(TUpdateModel, CancellationToken)"/> endpoint should be synchronized
	/// using the <see cref="IEntity{TEntityKey}.Id"/> of the entity being updated.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="false"/>.
	/// </remarks>
	protected virtual bool PutLock { get; }

	/// <summary>
	/// Gets a value indicating whether calls to the <see cref="Delete(TEntityKey, CancellationToken)"/> endpoint should be synchronized
	/// using the <see cref="IEntity{TEntityKey}.Id"/> of the entity being deleted.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="false"/>.
	/// </remarks>
	protected virtual bool DeleteLock { get; }

	/// <summary>
	/// Gets a value indicating whether result models created when the <see cref="Post(TCreateModel, CancellationToken)"/> endpoint is called
	/// should be automatically mapped from the created entity using the <see cref="UmbrellaDataAccessApiController.Mapper"/>.
	/// </summary>
	/// <remarks>
	/// <para>Defaults to <see langword="true"/>.</para>
	/// <para>
	/// This should normally always be set to <see langword="true"/>.
	/// It exists because output mapping was not supported by previous versions of this code.
	/// In future versions, this property will be removed with output mapping always being enabled.
	/// </para>
	/// </remarks>
	protected virtual bool EnablePostOutputMapping { get; } = true;

	/// <summary>
	/// Gets a value indicating whether result models created when the <see cref="Put(TUpdateModel, CancellationToken)"/> endpoint is called
	/// should be automatically mapped from the updated entity using the <see cref="UmbrellaDataAccessApiController.Mapper"/>.
	/// </summary>
	/// <remarks>
	/// <para>Defaults to <see langword="true"/>.</para>
	/// <para>
	/// This should normally always be set to <see langword="true"/>.
	/// It exists because output mapping was not supported by previous versions of this code.
	/// In future versions, this property will be removed with output mapping always being enabled.
	/// </para>
	/// </remarks>
	protected virtual bool EnablePutOutputMapping { get; } = true;

	protected virtual IncludeMap<TEntity>? SearchSlimIncludeMap { get; }
	protected virtual IncludeMap<TEntity>? GetIncludeMap { get; }
	protected virtual IncludeMap<TEntity>? PutIncludeMap { get; }
	protected virtual IncludeMap<TEntity>? DeleteIncludeMap { get; }

	protected virtual TRepositoryOptions? SearchSlimRepoOptions { get; }
	protected virtual TRepositoryOptions? GetRepoOptions { get; }
	protected virtual TRepositoryOptions? PostRepoOptions { get; }
	protected virtual TRepositoryOptions? PutRepoOptions { get; }
	protected virtual TRepositoryOptions? DeleteRepoOptions { get; }

	protected virtual RepoOptions[] SearchSlimChildRepoOptions { get; } = Array.Empty<RepoOptions>();
	protected virtual RepoOptions[] GetChildRepoOptions { get; } = Array.Empty<RepoOptions>();
	protected virtual RepoOptions[] PostChildRepoOptions { get; } = Array.Empty<RepoOptions>();
	protected virtual RepoOptions[] PutChildRepoOptions { get; } = Array.Empty<RepoOptions>();
	protected virtual RepoOptions[] DeleteChildRepoOptions { get; } = Array.Empty<RepoOptions>();

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaGenericRepositoryApiController{TSlimModel, TPaginatedResultModel, TModel, TCreateModel, TCreateResultModel, TUpdateModel, TUpdateResultModel, TRepository, TEntity, TRepositoryOptions, TEntityKey}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hostingEnvironment">The hosting environment.</param>
	/// <param name="options">The options.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	/// <param name="authorizationService">The authorization service.</param>
	/// <param name="synchronizationManager">The synchronization manager.</param>
	protected UmbrellaGenericRepositoryApiController(
		ILogger logger,
		IWebHostEnvironment hostingEnvironment,
		UmbrellaDataAccessApiControllerOptions options,
		IUmbrellaMapper mapper,
		Lazy<TRepository> repository,
		IAuthorizationService authorizationService,
		ISynchronizationManager synchronizationManager)
		: base(logger, hostingEnvironment, options, mapper, authorizationService, synchronizationManager)
	{
		Repository = repository;
	}

	[HttpGet(nameof(SearchSlim))]
	public virtual Task<IActionResult> SearchSlim(int pageNumber, int pageSize, [FromQuery] SortExpression<TEntity>[]? sorters = null, [FromQuery] FilterExpression<TEntity>[]? filters = null, FilterExpressionCombinator? filterCombinator = null, CancellationToken cancellationToken = default)
		=> SlimReadEndpointEnabled
		? ReadAllAsync<TEntity, TEntity, TEntityKey, TRepositoryOptions, TSlimModel, TPaginatedResultModel>(
			pageNumber,
			pageSize,
			sorters,
			filters,
			filterCombinator,
			cancellationToken,
			LoadSearchSlimDataAsync,
			null,
			AfterCreateSearchSlimModelAsync,
			AfterReadSlimEntityAsync,
			SearchSlimRepoOptions,
			SearchSlimChildRepoOptions,
			AuthorizationSlimReadChecksEnabled)
		: throw new NotSupportedException("Unsupported Endpoint");

	[HttpGet]
	public virtual Task<IActionResult> Get(TEntityKey id, CancellationToken cancellationToken = default)
		=> ReadEndpointEnabled
		? ReadAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel>(
			id,
			Repository,
			cancellationToken,
			null,
			(entity, model) => AfterReadEntityAsync(entity, model, cancellationToken),
			GetTrackChanges,
			GetIncludeMap,
			GetRepoOptions,
			GetChildRepoOptions,
			AuthorizationReadChecksEnabled,
			GetLock)
		: throw new NotSupportedException("Unsupported Endpoint");

	[HttpPost]
	public virtual Task<IActionResult> Post(TCreateModel model, CancellationToken cancellationToken = default)
		=> CreateEndpointEnabled
		? CreateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TCreateModel, TCreateResultModel>(
			model,
			Repository,
			cancellationToken,
			null,
			entity => BeforeCreateEntityAsync(entity, model, cancellationToken),
			null,
			(entity, result) => AfterCreateEntityAsync(entity, model, result, cancellationToken),
			PostRepoOptions,
			PostChildRepoOptions,
			AuthorizationCreateChecksEnabled,
			PostLock,
			EnablePostOutputMapping)
		: throw new NotSupportedException("Unsupported Endpoint");

	[HttpPut]
	public virtual Task<IActionResult> Put(TUpdateModel model, CancellationToken cancellationToken = default)
		=> UpdateEndpointEnabled
		? UpdateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TUpdateModel, TUpdateResultModel>(
			model,
			Repository,
			cancellationToken,
			null,
			entity => BeforeUpdateEntityAsync(entity, model, cancellationToken),
			null,
			(entity, result) => AfterUpdateEntityAsync(entity, model, result, cancellationToken),
			PutIncludeMap,
			PutRepoOptions,
			PutChildRepoOptions,
			AuthorizationUpdateChecksEnabled,
			PutLock,
			EnablePutOutputMapping)
		: throw new NotSupportedException("Unsupported Endpoint");

	[HttpDelete]
	public virtual Task<IActionResult> Delete(TEntityKey id, CancellationToken cancellationToken = default)
		=> DeleteEndpointEnabled
		? DeleteAsync(
			id,
			Repository,
			cancellationToken,
			BeforeDeleteEntityAsync,
			AfterDeleteEntityAsync,
			DeleteIncludeMap,
			DeleteRepoOptions,
			DeleteChildRepoOptions,
			AuthorizationDeleteChecksEnabled,
			DeleteLock)
		: throw new NotSupportedException("Unsupported Endpoint");

	protected virtual Task<PaginatedResultModel<TEntity>> LoadSearchSlimDataAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, SortExpression<TEntity>[]? sorters, FilterExpression<TEntity>[]? filters, FilterExpressionCombinator? filterCombinator, TRepositoryOptions? options, IEnumerable<RepoOptions>? childOptions) => Repository.Value.FindAllAsync(pageNumber, pageSize, cancellationToken, false, SearchSlimIncludeMap, sorters, filters, filterCombinator ?? FilterExpressionCombinator.And, options, childOptions);
	protected virtual Task AfterCreateSearchSlimModelAsync(PaginatedResultModel<TEntity> results, TPaginatedResultModel model, SortExpression<TEntity>[]? sorters, FilterExpression<TEntity>[]? filters, FilterExpressionCombinator? filterCombinator, CancellationToken cancellationToken) => Task.CompletedTask;
	protected virtual Task<IActionResult?> AfterReadSlimEntityAsync(TEntity entity, TSlimModel model, CancellationToken cancellationToken) => Task.FromResult<IActionResult?>(null);
	protected virtual Task<IActionResult?> AfterReadEntityAsync(TEntity entity, TModel model, CancellationToken cancellationToken) => Task.FromResult<IActionResult?>(null);
	protected virtual Task<IActionResult?> BeforeCreateEntityAsync(TEntity entity, TCreateModel model, CancellationToken cancellationToken) => Task.FromResult<IActionResult?>(null);
	protected virtual Task<IActionResult?> BeforeUpdateEntityAsync(TEntity entity, TUpdateModel model, CancellationToken cancellationToken) => Task.FromResult<IActionResult?>(null);
	protected virtual Task<IActionResult?> BeforeDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken) => Task.FromResult<IActionResult?>(null);
	protected virtual Task AfterCreateEntityAsync(TEntity entity, TCreateModel model, TCreateResultModel result, CancellationToken cancellationToken) => Task.CompletedTask;
	protected virtual Task AfterUpdateEntityAsync(TEntity entity, TUpdateModel model, TUpdateResultModel result, CancellationToken cancellationToken) => Task.CompletedTask;
	protected virtual Task AfterDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
}