// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Pagination;

namespace Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;

/// <summary>
/// A service that can be used with Blazor components that contain a <see cref="UmbrellaGrid{TItem}"/> component in conjunction with the <see cref="DataAccess.Remote"/> infrastructure.
/// Multiple instances of this service can be used to power multiple grids contained within a single Blazor component.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TRepository">The type of the repository.</typeparam>
/// <seealso cref="IUmbrellaGridComponentService{TItemModel, TPaginatedResultModel}" />
public interface IUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> : IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>
	where TItemModel : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
	where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>
{
	/// <summary>
	/// Gets the repository.
	/// </summary>
	TRepository Repository { get; }

	/// <summary>
	/// The event handler invoked when an item in the grid is to be deleted.
	/// </summary>
	/// <param name="item">The item.</param>
	Task DeleteItemClick(TItemModel item);
}