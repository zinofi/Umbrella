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
	/// Gets a value specifying whether or not the grid will automatically render when the page loads. If this is set to <see langword="false" />
	/// the <see cref="InitializeGridAsync"/> method should be manually called.
	/// </summary>
	/// <remarks>Defaults to <see langword="true"/>.</remarks>
	bool AutoRenderOnPageLoad { get; init; }

	/// <summary>
	/// Gets a value specifying whether or not the <see cref="GridInstance"/> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <see cref="UmbrellaGrid{TItem}.UpdateAsync(IReadOnlyCollection{TItem}, int?, int?, int?, bool)"/> method called internally
	/// by the <see cref="RefreshGridAsync(int, int, IEnumerable{SortExpressionDescriptor}?, IEnumerable{FilterExpressionDescriptor}?)"/> method.
	/// </summary>
	/// <remarks>Defaults to <see langword="true"/>.</remarks>
	bool CallGridStateHasChangedOnRefresh { get; init; }

	/// <summary>
	/// Gets the current refresh options which are updated when the grid options have been changed as a result of filtering, sorting or paginating through data.
	/// </summary>
	UmbrellaGridRefreshEventArgs? CurrentRefreshOptions { get; }

	/// <summary>
	/// Gets or sets the grid instance.
	/// </summary>
	UmbrellaGrid<TItemModel> GridInstance { get; set; }

	/// <summary>
	/// Gets the sort direction used to initially sort the data in the grid.
	/// </summary>
	/// <remarks>Defaults to <see cref="SortDirection.Descending"/>.</remarks>
	SortDirection InitialSortDirection { get; init; }

	/// <summary>
	/// Gets the collection of sort expressions used to initially sort the data in the grid.
	/// </summary>
	/// <remarks>
	/// Defaults to a collection containing a single sort expresssion which uses the <see cref="InitialSortPropertyName"/> and <see cref="InitialSortDirection"/>.
	/// </remarks>
	Lazy<IReadOnlyCollection<SortExpressionDescriptor>> InitialSortExpressions { get; }

	/// <summary>
	/// Gets the collection of filter expressions used to initially filter the data in the grid.
	/// </summary>
	/// <remarks>
	/// Defaults to an empty array.
	/// </remarks>
	IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; }

	/// <summary>
	/// Gets the property name used to initally sort the data in the grid.
	/// </summary>
	string InitialSortPropertyName { get; init; }

	/// <summary>
	/// Gets the delegate used to load the paginated results.
	/// </summary>
	Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>> LoadPaginatedResultModelDelegate { get; }

	/// <summary>
	/// Gets the StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.
	/// </summary>
	Action StateHasChangedDelegate { get; init; }

	/// <summary>
	/// Initializes the grid.
	/// </summary>
	Task InitializeGridAsync();

	/// <summary>
	/// This should be called from within the <see cref="ComponentBase.OnAfterRenderAsync(bool)"/> method of the Blazor component this service is being used with.
	/// </summary>
	/// <param name="firstRender">
	/// Set to <see langword="true"/> if this is the first time <see cref="ComponentBase.OnAfterRenderAsync(bool)"/>
	/// has been invoked on the component instance; otherwise <see langword="false"/>.</param>
	/// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
	Task OnAfterRenderAsync(bool firstRender);

	/// <summary>
	/// The event handler invoked by the <see cref="GridInstance"/> as a result of filtering, sorting or pagination.
	/// </summary>
	/// <param name="args">The event args containing details of the current filtering, sorting and pagination options.</param>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	Task OnGridOptionsChangedAsync(UmbrellaGridRefreshEventArgs args);

	/// <summary>
	/// Refreshes the data in the grid using the specified options. Internally this calls <see cref="LoadPaginatedResultModelDelegate" />
	/// to load new data, usually from the server.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <returns>An awaitable Task that completes when this operation has completed.</returns>
	Task RefreshGridAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null);

	/// <summary>
	/// Refreshes the grid using any existing <see cref="CurrentRefreshOptions"/> falling back to the
	/// <see cref="UmbrellaGrid{TItem}.PageNumber"/> and <see cref="UmbrellaGrid{TItem}.PageSize"/> for the current <see cref="GridInstance"/>
	/// where needed.
	/// </summary>
	/// <returns>An awaitable Task that completes when this operation has completed.</returns>
	Task RefreshGridAsyncUsingCurrentRefreshOptionsAsync();
}