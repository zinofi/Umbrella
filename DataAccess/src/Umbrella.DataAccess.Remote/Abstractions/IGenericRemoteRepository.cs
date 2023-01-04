// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.DataAnnotations.Enumerations;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.DataAccess.Remote.Abstractions;

/// <summary>
/// A generic repository used to query slim paginated remote resources.
/// </summary>
/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
public interface IReadOnlyPaginatedSlimItemGenericRemoteRepository<TSlimItem, TIdentifier, TPaginatedResultModel>
	where TSlimItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
{
	/// <summary>
	/// Finds all slim results of <typeparamref name="TSlimItem"/> on the server using the specified parameters.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <param name="filterCombinator">The filter combinator.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IHttpCallResult<TPaginatedResultModel?>> FindAllSlimAsync(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.And, CancellationToken cancellationToken = default);
}

/// <summary>
/// A generic repository used to query a remote resource.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public interface IReadOnlyGenericRemoteRepository<TItem, TIdentifier>
	where TItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <summary>
	/// Determines if the specified resource exists on the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IHttpCallResult<bool>> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds the specified resource on the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IHttpCallResult<TItem?>> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds the total count of <typeparamref name="TItem"/> that exist on the remote server.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IHttpCallResult<int>> FindTotalCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A generic repository used to delete remote resources.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public interface IDeleteItemGenericRemoteRepository<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <summary>
	/// Deletes the specified resource from the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IHttpCallResult> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);
}

/// <summary>
/// A generic repository used to query and update a remote resource.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TCreateItem">The type of the create item.</typeparam>
/// <typeparam name="TCreateResult">The type of the create result.</typeparam>
/// <typeparam name="TUpdateItem">The type of the update item.</typeparam>
/// <typeparam name="TUpdateResult">The type of the update result.</typeparam>
public interface IGenericRemoteRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult> : IReadOnlyGenericRemoteRepository<TItem, TIdentifier>, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TSlimItem, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>
	where TItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TSlimItem : class, IKeyedItem<TIdentifier>
	where TUpdateItem : class, IKeyedItem<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
{
	/// <summary>
	/// Creates the specified resource on the remote server.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<(IHttpCallResult<TCreateResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> CreateAsync(TCreateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the specified resource on the remote server.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<(IHttpCallResult<TUpdateResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> UpdateAsync(TUpdateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default);
}