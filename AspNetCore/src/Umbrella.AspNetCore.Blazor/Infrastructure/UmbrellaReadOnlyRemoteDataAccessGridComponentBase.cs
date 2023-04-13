// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with Blazor components that contain a single <see cref="UmbrellaGrid{TItem}"/> component in conjunction
/// with the <see cref="DataAccess.Remote"/> infrastructure.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TRepository">The type of the repository.</typeparam>
/// <seealso cref="UmbrellaGridComponentBase{TItemModel, TPaginatedResultModel}" />
public abstract class UmbrellaReadOnlyRemoteDataAccessGridComponentBase<TItemModel, TPaginatedResultModel, TRepository> : UmbrellaGridComponentBase<TItemModel, TPaginatedResultModel>
	where TItemModel : class
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
	where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TPaginatedResultModel>
{
	/// <summary>
	/// Gets or sets the repository.
	/// </summary>
	[Inject]
	protected TRepository Repository { get; set; } = null!;

	/// <inheritdoc/>
	protected override Task<IHttpCallResult<TPaginatedResultModel?>> LoadPaginatedResultModelAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null) => Repository.FindAllSlimAsync(pageNumber, pageSize, sorters: sorters, filters: filters);
}
