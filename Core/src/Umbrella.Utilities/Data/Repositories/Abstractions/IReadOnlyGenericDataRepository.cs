// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Data.Repositories.Abstractions;

/// <summary>
/// A generic repository used to query a remote resource.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public interface IReadOnlyGenericDataRepository<TItem, TIdentifier>
	where TItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <summary>
	/// Determines if the specified resource exists on the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IOperationResult<bool>> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds the specified resource on the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IOperationResult<TItem?>> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finds the total count of <typeparamref name="TItem"/> that exist on the remote server.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IOperationResult<int>> FindTotalCountAsync(CancellationToken cancellationToken = default);
}
