// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Shared.Models;
using Umbrella.AspNetCore.WebUtilities.Mvc.Options;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
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
		protected Lazy<TRepository> Repository { get; }
		protected virtual bool AuthorizationSlimReadChecksEnabled { get; } = true;
		protected virtual bool AuthorizationReadChecksEnabled { get; } = true;
		protected virtual bool AuthorizationCreateChecksEnabled { get; } = true;
		protected virtual bool AuthorizationUpdateChecksEnabled { get; } = true;
		protected virtual bool AuthorizationDeleteChecksEnabled { get; } = true;

		protected virtual bool GetTrackChanges { get; }

		protected virtual bool GetLock { get; }
		protected virtual bool PostLock { get; }
		protected virtual bool PutLock { get; }
		protected virtual bool DeleteLock { get; }

		protected virtual bool EnablePostOutputMapping { get; } = true;
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

		protected UmbrellaGenericRepositoryApiController(
			ILogger logger,
			IWebHostEnvironment hostingEnvironment,
			UmbrellaDataAccessApiControllerOptions options,
			IMapper mapper,
			Lazy<TRepository> repository,
			IAuthorizationService authorizationService,
			ISynchronizationManager synchronizationManager)
			: base(logger, hostingEnvironment, options, mapper, authorizationService, synchronizationManager)
		{
			Repository = repository;
		}

		[HttpGet(nameof(SearchSlim))]
		public virtual Task<IActionResult> SearchSlim(int pageNumber, int pageSize, [FromQuery] SortExpression<TEntity>[]? sorters = null, [FromQuery] FilterExpression<TEntity>[]? filters = null, FilterExpressionCombinator? filterCombinator = null, CancellationToken cancellationToken = default)
			=> ReadAllAsync<TEntity, TEntity, TEntityKey, TRepositoryOptions, TSlimModel, TPaginatedResultModel>(
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
				AuthorizationSlimReadChecksEnabled);

		[HttpGet]
		public virtual Task<IActionResult> Get(TEntityKey id, CancellationToken cancellationToken = default)
			=> ReadAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TModel>(
				id,
				Repository,
				cancellationToken,
				null,
				(entity, model) => AfterReadEntityAsync(entity, model, cancellationToken),
				GetTrackChanges,
				GetIncludeMap,
				GetRepoOptions,
				AuthorizationReadChecksEnabled,
				GetLock);

		[HttpPost]
		public virtual Task<IActionResult> Post(TCreateModel model, CancellationToken cancellationToken = default)
			=> CreateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TCreateModel, TCreateResultModel>(
				model,
				Repository,
				cancellationToken,
				null,
				entity => BeforeCreateEntityAsync(entity, model, cancellationToken),
				null,
				(entity, result) => AfterCreateEntityAsync(entity, model, result, cancellationToken),
				PostRepoOptions,
				AuthorizationCreateChecksEnabled,
				PostLock,
				EnablePostOutputMapping);

		[HttpPut]
		public virtual Task<IActionResult> Put(TUpdateModel model, CancellationToken cancellationToken = default)
			=> UpdateAsync<TEntity, TEntityKey, TRepository, TRepositoryOptions, TUpdateModel, TUpdateResultModel>(
				model,
				Repository,
				cancellationToken,
				null,
				entity => BeforeUpdateEntityAsync(entity, model, cancellationToken),
				null,
				(entity, result) => AfterUpdateEntityAsync(entity, model, result, cancellationToken),
				PutIncludeMap,
				PutRepoOptions,
				AuthorizationUpdateChecksEnabled,
				PutLock,
				EnablePutOutputMapping);

		[HttpDelete]
		public virtual Task<IActionResult> Delete(TEntityKey id, CancellationToken cancellationToken = default)
			=> DeleteAsync(
				id,
				Repository,
				cancellationToken,
				BeforeDeleteEntityAsync,
				AfterDeleteEntityAsync,
				DeleteIncludeMap,
				DeleteRepoOptions,
				AuthorizationDeleteChecksEnabled,
				DeleteLock);

		protected virtual Task<PaginatedResultModel<TEntity>> LoadSearchSlimDataAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, SortExpression<TEntity>[]? sorters, FilterExpression<TEntity>[]? filters, FilterExpressionCombinator? filterCombinator, TRepositoryOptions? options) => Repository.Value.FindAllAsync(pageNumber, pageSize, cancellationToken, false, SearchSlimIncludeMap, sorters, filters, filterCombinator ?? FilterExpressionCombinator.Or, options);
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
}