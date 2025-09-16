using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Data.Services.Abstractions;

/// <summary>
/// A generic service used to query and update a resource.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TCreateItem">The type of the create item.</typeparam>
/// <typeparam name="TCreateResult">The type of the create result.</typeparam>
/// <typeparam name="TUpdateItem">The type of the update item.</typeparam>
/// <typeparam name="TUpdateResult">The type of the update result.</typeparam>
public interface IGenericDataService<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult> : IReadOnlyGenericDataService<TItem, TIdentifier>, IReadOnlyPaginatedSlimItemGenericDataRepository<TSlimItem, TPaginatedResultModel>, IDeleteItemGenericDataService<TIdentifier>
	where TItem : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TSlimItem : class, IKeyedItem<TIdentifier>
	where TUpdateItem : class, IKeyedItem<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
{
	/// <summary>
	/// Creates the specified resource.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IOperationResult<TCreateResult?>> CreateAsync(TCreateItem item, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the specified resource.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IOperationResult<TUpdateResult?>> UpdateAsync(TUpdateItem item, CancellationToken cancellationToken = default);
}