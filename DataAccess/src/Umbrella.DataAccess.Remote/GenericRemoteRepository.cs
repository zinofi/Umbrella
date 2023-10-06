// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.DataAccess.Remote.Exceptions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.DataAnnotations.Abstractions;
using Umbrella.Utilities.DataAnnotations.Enumerations;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.DataAccess.Remote;

/// <summary>
/// A generic repository used to query and update a remote resource over HTTP.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TCreateItem">The type of the create item.</typeparam>
/// <typeparam name="TCreateResult">The type of the create result.</typeparam>
/// <typeparam name="TUpdateItem">The type of the update item.</typeparam>
/// <typeparam name="TUpdateResult">The type of the update result.</typeparam>
public abstract class GenericRemoteRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult> : GenericRemoteDataService, IGenericRemoteRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult>
	where TItem : class, IKeyedItem<TIdentifier>
	where TSlimItem : class, IKeyedItem<TIdentifier>
	where TUpdateItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
{
	/// <summary>
	/// The endpoint called by the <see cref="FindAllSlimAsync(int, int, IEnumerable{SortExpressionDescriptor}?, IEnumerable{FilterExpressionDescriptor}?, FilterExpressionCombinator, CancellationToken)"/>
	/// method.
	/// </summary>
	/// <remarks>
	/// Defaults to /SearchSlim. This path must start with a leading /.
	/// </remarks>
	protected virtual string FindAllSlimEndpoint { get; } = "/SearchSlim";

	/// <summary>
	/// Initializes a new instance of the <see cref="GenericRemoteRepository{TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="httpService">The HTTP service.</param>
	/// <param name="httpServiceUtility">The HTTP service utility.</param>
	/// <param name="validator">The validator.</param>
	public GenericRemoteRepository(
		ILogger logger,
		IGenericHttpService httpService,
		IGenericHttpServiceUtility httpServiceUtility,
		IUmbrellaValidator validator) : base(logger, httpService, httpServiceUtility, validator)
	{
	}

	/// <inheritdoc />
	public virtual Task<IHttpCallResult<TItem?>> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		=> GetByIdAsync<TItem, TIdentifier>(id, cancellationToken, AfterItemLoadedAsync);

	/// <inheritdoc />
	public virtual Task<IHttpCallResult<TPaginatedResultModel?>> FindAllSlimAsync(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.And, CancellationToken cancellationToken = default)
		=> GetAllSlimAsync<TPaginatedResultModel, TSlimItem, TIdentifier>(FindAllSlimEndpoint, pageNumber, pageSize, sorters, filters, filterCombinator, AfterAllItemsLoadedAsync, cancellationToken);

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<int>> FindTotalCountAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return await RemoteService.GetAsync<int>(ApiUrl + "/Count", cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem finding the total count.", exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<bool>> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(id);

		try
		{
			string? strId = id.ToString();

			if (string.IsNullOrEmpty(strId))
				throw new InvalidOperationException("The id cannot be converted to a string.");

			var parameters = new Dictionary<string, string>
			{
				["id"] = strId
			};

			return await RemoteService.GetAsync<bool>(ApiUrl + "/Exists", parameters, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { id }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem determining if the specified item exists.", exc);
		}
	}

	/// <inheritdoc />
	public virtual Task<(IHttpCallResult<TCreateResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> CreateAsync(TCreateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default)
		=> PostAsync<TCreateItem, TCreateResult>(item, sanitize, validationType, AfterItemCreatedAsync, cancellationToken: cancellationToken);

	/// <inheritdoc />
	public virtual Task<(IHttpCallResult<TUpdateResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> UpdateAsync(TUpdateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default)
		=> PutAsync<TUpdateItem, TIdentifier, TUpdateResult>(item, sanitize, validationType, AfterItemUpdatedAsync, cancellationToken: cancellationToken);

	/// <inheritdoc />
	public virtual Task<IHttpCallResult> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default)
		=> DeleteAsync(id, AfterItemDeletedAsync, cancellationToken: cancellationToken);

	/// <summary>
	/// Override this in a derived type to perform an operation on the item after it has been loaded.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
	protected virtual Task AfterItemLoadedAsync(TItem item, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}

	/// <summary>
	///  Override this in a derived type to perform an operation on the items after they have been loaded.
	/// </summary>
	/// <param name="items">The items.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	protected virtual Task AfterAllItemsLoadedAsync(IEnumerable<TSlimItem> items, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}

	/// <summary>
	/// Overriding this method allows you to perform any work after the item has been created.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="result">The result.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
	protected virtual Task AfterItemCreatedAsync(TCreateItem item, TCreateResult? result, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}

	/// <summary>
	/// Overriding this method allows you to perform any work after the item has been updated.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="result">The result.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
	protected virtual Task AfterItemUpdatedAsync(TUpdateItem item, TUpdateResult? result, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}

	/// <summary>
	/// Overriding this method allows you to perform any work after the item has been deleted.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
	protected virtual Task AfterItemDeletedAsync(TIdentifier id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}
}