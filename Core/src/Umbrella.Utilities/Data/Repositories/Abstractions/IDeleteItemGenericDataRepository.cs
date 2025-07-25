// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Data.Repositories.Abstractions;

/// <summary>
/// A generic repository used to delete remote resources.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public interface IDeleteItemGenericDataRepository<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <summary>
	/// Deletes the specified resource from the remote server.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the remote operation.</returns>
	Task<IOperationResult> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);
}
