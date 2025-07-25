// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.DataAnnotations.Enumerations;
using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Data.Repositories.Abstractions;

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
public interface IGenericDataRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult> : IReadOnlyGenericDataRepository<TItem, TIdentifier>, IReadOnlyPaginatedSlimItemGenericDataRepository<TSlimItem, TPaginatedResultModel>, IDeleteItemGenericDataRepository<TIdentifier>
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
	Task<IOperationResult<TCreateResult?>> CreateAsync(TCreateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the specified resource on the remote server.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IOperationResult<TUpdateResult?>> UpdateAsync(TUpdateItem item, bool sanitize = true, ValidationType validationType = ValidationType.Shallow, CancellationToken cancellationToken = default);
}