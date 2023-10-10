using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;

/// <summary>
/// A service that can be used with Blazor components that contain a <see cref="UmbrellaGrid{TItem}"/> component.
/// Multiple instances of this service can be used to power multiple grids contained within a single Blazor component.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
public interface IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>
	where TItemModel : notnull
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
{
	/// <summary>
	/// Gets a value specifying whether or not the <see cref="GridInstance"/> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component responds to an <see cref="UmbrellaGridDataRequest"/> and provides a <see cref="UmbrellaGridDataResponse{TItem}"/>.
	/// </summary>
	/// <remarks>Defaults to <see langword="true"/>.</remarks>
	bool CallGridStateHasChangedOnRefresh { get; init; }

	/// <summary>
	/// Gets or sets the grid instance.
	/// </summary>
	UmbrellaGrid<TItemModel> GridInstance { get; set; }

	/// <summary>
	/// Gets the collection of filter expressions used to initially filter the data in the grid.
	/// </summary>
	/// <remarks>
	/// Defaults to an empty array.
	/// </remarks>
	IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; }

	/// <summary>
	/// Gets the delegate used to load the paginated results.
	/// </summary>
	Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, CancellationToken, Task<IHttpCallResult<TPaginatedResultModel?>>> LoadPaginatedResultModelDelegate { get; }

	/// <summary>
	/// Gets the StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.
	/// </summary>
	Action StateHasChangedDelegate { get; init; }

	/// <summary>
	/// Invoked by the <see cref="GridInstance"/> when it requests new data.
	/// </summary>
	/// <param name="args">The request containing details of the current filtering, sorting and pagination options.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The response provided to the grid.</returns>
	Task<UmbrellaGridDataResponse<TItemModel>?> OnGridDataRequestAsync(UmbrellaGridDataRequest args, CancellationToken cancellationToken);
}