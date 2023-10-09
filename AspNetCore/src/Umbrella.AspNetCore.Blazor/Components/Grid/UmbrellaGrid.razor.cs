using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs;
using Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs.Models;
using Umbrella.AspNetCore.Blazor.Components.Pagination;
using Umbrella.AspNetCore.Blazor.Enumerations;
using Umbrella.AspNetCore.Blazor.Extensions;
using Umbrella.AspNetCore.Blazor.Services.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Dating;
using Umbrella.Utilities.Dating.Json;

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

// TODO: Maybe it's better to create a new CollectionView control here. This Grid adds nothing more than a loop and some basic structure for that scenario.
// Could even have some kind of base component that encapsulates core functionality, e.g. auto-scroll.
// TODO: Create a PaginationMode option to replace ShowPagination: None, Top, Bottom, Both. Do the same for caption. Default to Bottom.

/// <summary>
/// The rendering mode the <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
public enum UmbrellaGridRenderMode
{
	/// <summary>
	/// Full rendering mode where the grid is rendered as a table.
	/// </summary>
	Table,

	/// <summary>
	/// Slim rendering mode the grid and its columns are rendered using divs in order to allow non-tabular layouts to be used.
	/// </summary>
	CollectionView
}

/// <summary>
/// A grid component used to display bound items as a series of columns in a grid.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="ComponentBase" />
/// <seealso cref="IUmbrellaGrid{TItem}" />
[CascadingTypeParameter(nameof(TItem))]
public partial class UmbrellaGrid<TItem> : IUmbrellaGrid<TItem>
	where TItem : notnull
{
	private class UmbrellaGridSelectableItem
	{
		public UmbrellaGridSelectableItem(bool isSelected, TItem item)
		{
			IsSelected = isSelected;
			Item = item;
		}

		public bool IsSelected { get; set; }
		public TItem Item { get; }
	}

	private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
	private bool _autoScrollEnabled;
	private EditContext EditContext { get; } = new(new object());

	[Inject]
	private ILogger<UmbrellaGrid<TItem>> Logger { get; set; } = null!;

	[Inject]
	private IUmbrellaBlazorInteropService BlazorInteropUtility { get; set; } = null!;

	[Inject]
	private IUmbrellaDialogService DialogService { get; set; } = null!;

	[Inject]
	private NavigationManager Navigation { get; set; } = null!;

	/// <summary>
	/// Gets or sets the instance of the associated <see cref="UmbrellaPagination"/> component.
	/// </summary>
	private UmbrellaPagination PaginationInstance { get; set; } = null!;

	/// <summary>
	/// Gets or sets a value indicating whether the scanning process that occurs when the grid is first initialized, which looks for columns, has been completed.
	/// </summary>
	private bool ColumnScanComplete { get; set; }

	/// <summary>
	/// Gets the columns.
	/// </summary>
	private List<IUmbrellaColumnDefinition<TItem>> ColumnDefinitions { get; } = new();

	private List<UmbrellaGridSelectableItem> SelectableItems { get; } = new();

	private TItem SelectedRow { get; set; } = default!;

	/// <summary>
	/// Gets or sets a value indicating whether an item has been selected using a row checkbox.
	/// </summary>
	public bool CheckboxSelectColumnSelected { get; private set; }

	/// <inheritdoc />
	public string? FirstColumnPropertyName { get; private set; }

	/// <summary>
	/// Gets the filterable columns.
	/// </summary>
	private IReadOnlyCollection<IUmbrellaColumnDefinition<TItem>>? FilterableColumns { get; set; }

	/// <summary>
	/// Gets the current <see cref="LayoutState"/> of the component.
	/// </summary>
	private LayoutState CurrentState { get; set; } = LayoutState.Loading;

	/// <summary>
	/// Gets or sets the total count of all items that are available to be loaded into the grid.
	/// This will be larger than the number of items currently displayed where pagination has been applied.
	/// </summary>
	private int TotalCount { get; set; }

	/// <summary>
	/// Gets the current page number. Defaults to <see cref="UmbrellaPaginationDefaults.PageNumber"/>.
	/// </summary>
	public int PageNumber { get; private set; } = UmbrellaPaginationDefaults.PageNumber;

	/// <summary>
	/// Gets the current page size. Defaults to <see cref="UmbrellaPaginationDefaults.PageSize"/>.
	/// </summary>
	public int PageSize { get; private set; } = UmbrellaPaginationDefaults.PageSize;

	/// <summary>
	/// Gets or sets the additional content to be displayed between the filter fields
	/// and the action buttons that apply the filters to the grid.
	/// </summary>
	[Parameter]
	public RenderFragment? AdditionalFilterContent { get; set; }

	/// <summary>
	/// Gets or sets the columns to be displayed inside this grid component. This should be a collection of columns components.
	/// </summary>
	[Parameter]
	public RenderFragment<TItem>? Columns { get; set; }

	/// <summary>
	/// Gets or sets the bulk actions that can be performed on items in this grid.
	/// </summary>
	[Parameter]
	public RenderFragment? BulkActions { get; set; }

	/// <summary>
	/// Gets or sets the items collection displayed in the grid.
	/// </summary>
	public IReadOnlyCollection<TItem> Items { get; private set; } = Array.Empty<TItem>();

	/// <summary>
	/// Gets or sets the message displayed when the grid is loading.
	/// This is ususally only shown when the grid is first initialized.
	/// </summary>
	/// <remarks>Defaults to <c>Loading... Please wait.</c></remarks>
	[Parameter]
	public string LoadingMessage { get; set; } = "Loading... Please wait.";

	/// <summary>
	/// Gets or sets the content displayed when the grid is loading.
	/// This is ususally only shown when the grid is first initialized.
	/// This overrides the value of the <see cref="LoadingMessage"/> when specified.
	/// </summary>
	[Parameter]
	public RenderFragment? LoadingMessageContent { get; set; }

	/// <summary>
	/// Gets or sets the message displayed when the grid contains no data.
	/// </summary>
	/// <remarks>Defaults to <c>There is either no data to display or your search options have no results.</c></remarks>
	[Parameter]
	public string EmptyMessage { get; set; } = "There is either no data to display or your search options have no results.";

	/// <summary>
	/// Gets or sets the content displayed when there are no results.
	/// This overrides the value of the <see cref="EmptyMessage"/> when specified.
	/// </summary>
	/// <remarks>Defaults to <c>There has been a problem. Please try again.</c></remarks>
	[Parameter]
	public RenderFragment? EmptyMessageContent { get; set; }

	/// <summary>
	/// Gets or sets the error message displayed when the grid fails to load correctly.
	/// </summary>
	[Parameter]
	public string ErrorMessage { get; set; } = "There has been a problem. Please try again.";

	/// <summary>
	/// Gets or sets the content displayed when the grid fails to load correctly.
	/// This overrides the value of the <see cref="ErrorMessage"/> when specified.
	/// </summary>
	[Parameter]
	public RenderFragment? ErrorMessageContent { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the reload button should be shown when the grid has failed to load correctly. Defaults to <see langword="true"/>.
	/// </summary>
	[Parameter]
	public bool ShowReloadButton { get; set; } = true;

	/// <summary>
	/// Gets or sets the heading displayed above the filter options.
	/// </summary>
	[Parameter]
	public string FilterOptionsHeading { get; set; } = "Search Options";

	/// <summary>
	/// Gets or sets a value indicating whether the filter options, if there are any and <see cref="ShowFilters"/> is <see langword="true" />,
	/// are expanded when the grid is first loaded.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="true" />
	/// </remarks>
	[Parameter]
	public bool ExpandFiltersOnLoad { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether filter options should be displayed.
	/// </summary>
	[Parameter]
	public bool ShowFilters { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether pagination controls should be displayed.
	/// </summary>
	[Parameter]
	public bool ShowPagination { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the grid caption showing the range of results metadata should be displayed.
	/// </summary>
	[Parameter]
	public bool ShowCaption { get; set; } = true;

	/// <inheritdoc />
	[Parameter]
	public UmbrellaGridRenderMode RenderMode { get; set; } = UmbrellaGridRenderMode.Table;

	/// <summary>
	/// Gets or sets the name of the property which will be used to sort the data when the grid is first initialized.
	/// </summary>
	[Parameter]
	public string InitialSortPropertyName { get; set; } = null!;

	/// <summary>
	/// Gets or sets the initial sort direction. Defaults to <see cref="SortDirection.Descending"/>.
	/// </summary>
	[Parameter]
	public SortDirection InitialSortDirection { get; set; } = SortDirection.Descending;

	/// <summary>
	/// Gets or sets the initial filter expressions. Defaults to an empty array.
	/// </summary>
	[Parameter]
	public IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; set; } = Array.Empty<FilterExpressionDescriptor>();

	/// <summary>
	/// Gets or sets the event callback invoked by this component when its filtering, sorting or pagination state has changed.
	/// </summary>
	[Parameter]
	public EventCallback<UmbrellaGridRefreshEventArgs> OnGridOptionsChanged { get; set; }

	/// <summary>
	/// Gets or sets the page size options. Defaults to <see cref="UmbrellaPaginationDefaults.PageSizeOptions"/>.
	/// </summary>
	[Parameter]
	public IReadOnlyCollection<int> PageSizeOptions { get; set; } = UmbrellaPaginationDefaults.PageSizeOptions;

	/// <summary>
	/// Gets or sets the CSS class applied to the grid.
	/// </summary>
	[Parameter]
	public string? GridCssClass { get; set; } = "table table-hover";

	/// <summary>
	/// Gets or sets the CSS class applied to each item in the grid.
	/// </summary>
	[Parameter]
	public string? ItemCssClass { get; set; }

	/// <summary>
	/// Gets or sets the CSS class applied to selected items within the grid.
	/// </summary>
	[Parameter]
	public string? ItemSelectedCssClass { get; set; } = "table-primary";

	/// <summary>
	/// Gets or sets a value indicating whether the pagination controls should be rendered using smaller styling.
	/// </summary>
	[Parameter]
	public bool SmallPagination { get; set; } = true;

	/// <summary>
	/// Gets or sets whether the grid should auto-scroll to the top when it is updated with new data.
	/// </summary>
	[Parameter]
	public bool AutoScrollTop { get; set; } = true;

	/// <summary>
	/// Gets or sets the scroll offset from the top of the screen when the grid is auto-scrolled to the top if enabled using
	/// the <see cref="AutoScrollTop"/> property.
	/// </summary>
	/// <remarks>
	/// This exists to allow for things liked fixed navigation bars to be taken into account.
	/// </remarks>
	[Parameter]
	public int ScrollTopOffset { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether checkboxes should be shown at the start of each row.
	/// </summary>
	/// <remarks>
	/// Checkboxes on each row are used to select multiple items which can be used in conjunction with items rendered using the <see cref="BulkActions" /> property.
	/// </remarks>
	[Parameter]
	public bool ShowCheckboxSelectColumn { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether radio buttons should be shown at the start of each row.
	/// </summary>
	/// <remarks>
	/// This is primarily designed to allow a single row to be selected and highlighted by a user when the grid requires horizontal scrolling.
	/// This will help a user to track a single selected row.
	/// </remarks>
	[Parameter]
	public bool ShowRadioSelectColumn { get; set; }

	/// <summary>
	/// Gets or sets the callback that will be invoked when the search filters and sorters are being reset.
	/// </summary>
	[Parameter]
	public EventCallback OnResetFiltersAndSorters { get; set; }

	private void OnCheckboxSelectColumnSelectionChanged(UmbrellaGridSelectableItem selectableItem)
	{
		selectableItem.IsSelected = !selectableItem.IsSelected;
		CheckboxSelectColumnSelected = SelectableItems.Any(x => x.IsSelected);
	}

	private void OnCheckboxSelectColumnSelectedChanged()
	{
		CheckboxSelectColumnSelected = !CheckboxSelectColumnSelected;
		SelectableItems.ForEach(x => x.IsSelected = CheckboxSelectColumnSelected);
	}

	/// <summary>
	/// Gets the caption text.
	/// </summary>
	private string CaptionText
	{
		get
		{
			int startItem = ((PageNumber - 1) * PageSize) + 1;
			int endItem = Math.Min(PageSize, Items.Count) + (PageNumber == 1 ? 0 : startItem - 1);

			return TotalCount == 1 ? "Showing 1 of 1 items" : $"Showing items {startItem} to {endItem} of {TotalCount}";
		}
	}

	/// <inheritdoc/>
	protected override void OnParametersSet()
	{
		Guard.IsNotNullOrWhiteSpace(InitialSortPropertyName);
	}

	/// <inheritdoc />
	public void AddColumnDefinition(IUmbrellaColumnDefinition<TItem> column)
	{
		if (Logger.IsEnabled(LogLevel.Debug))
			Logger.WriteDebug(new { column });

		ColumnDefinitions.Add(column);
	}

	/// <inheritdoc />
	public async ValueTask SetColumnScanCompletedAsync()
	{
		if (!ColumnScanComplete)
		{
			ColumnScanComplete = true;

			if (Logger.IsEnabled(LogLevel.Debug))
				Logger.WriteDebug(new { ColumnScanComplete });

			var filterableColumns = new List<IUmbrellaColumnDefinition<TItem>>();

			if (Logger.IsEnabled(LogLevel.Debug))
				Logger.WriteDebug(new { ColumnDefinitionsCount = ColumnDefinitions.Count });

			for (int i = 0; i < ColumnDefinitions.Count; i++)
			{
				var column = ColumnDefinitions[i];

				if (column.DisplayMode == UmbrellaColumnDisplayMode.None)
					continue;

				if (i is 0)
					FirstColumnPropertyName = column.PropertyName;

				if (column.Filterable)
					filterableColumns.Add(column);

				if (InitialSortPropertyName == column.PropertyName)
					column.Direction = InitialSortDirection;

				if (InitialFilterExpressions.Count > 0)
				{
					string? memberPath = column.FilterMemberPathOverride ?? column.PropertyName;

					if (!string.IsNullOrEmpty(memberPath))
					{
						string? value = InitialFilterExpressions.FindFilterValue<string?>(memberPath);

						if (value is not null)
							column.FilterValue = value;
					}
				}
			}

			FilterableColumns = filterableColumns;

			if (Logger.IsEnabled(LogLevel.Debug))
				Logger.WriteDebug(new { FilterableColumnsCount = FilterableColumns.Count });

			await ApplyQueryStringSortersAndFiltersAsync();

			StateHasChanged();
		}
	}

	/// <summary>
	/// Changes the <see cref="CurrentState"/> to <see cref="LayoutState.Error"/> and forces re-rendering.
	/// </summary>
	public void SetErrorState()
	{
		CurrentState = LayoutState.Error;
		StateHasChanged();
	}

	/// <summary>
	/// Updates the grid data.
	/// </summary>
	/// <param name="items">The data items.</param>
	/// <param name="totalCount">The total size of all results without pagination applied.</param>
	/// <param name="pageNumber">The current page number.</param>
	/// <param name="pageSize">The current page size.</param>
	/// <param name="callStateHasChanged">Specifies whether <see cref="ComponentBase.StateHasChanged"/> should be invoked.</param>
	public async ValueTask UpdateAsync(IReadOnlyCollection<TItem> items, int? totalCount = null, int? pageNumber = null, int? pageSize = null, bool callStateHasChanged = true)
	{
		Items = items;
		TotalCount = totalCount ?? TotalCount;
		PageSize = pageSize ?? PageSize;
		PageNumber = pageNumber ?? PageNumber;

		SelectableItems.Clear();
		SelectableItems.AddRange(Items.Select(x => new UmbrellaGridSelectableItem(false, x)));
		SelectedRow = default!;
		CheckboxSelectColumnSelected = false;

		if (AutoScrollTop && _autoScrollEnabled)
		{
			await BlazorInteropUtility.AnimateScrollToAsync(".u-grid", ScrollTopOffset);
		}

		// Only enable auto-scrolling after the initial page load.
		_autoScrollEnabled = true;

		CurrentState = Items.Count > 0 ? LayoutState.Success : LayoutState.Empty;

		if (callStateHasChanged)
		{
			StateHasChanged();
		}
	}

	/// <summary>
	/// Gets a collection of the items that have been selected determined by the checked status of the checkboxes rendered on each
	/// row of the grid when <see cref="ShowCheckboxSelectColumn"/> is set to <see langword="true"/>.
	/// </summary>
	/// <returns>A collection of the selected items.</returns>
	public IReadOnlyCollection<TItem> GetSelectedItems() => SelectableItems.Where(x => x.IsSelected).Select(x => x.Item).ToArray();

	/// <summary>
	/// Gets the selected item.
	/// </summary>
	public TItem GetSelectedItem() => SelectedRow;

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender && PaginationInstance is not null)
		{
			await PaginationInstance.UpdateAsync(TotalCount, PageNumber, PageSize);
		}
	}

	/// <summary>
	/// The click event handler for the apply filters button.
	/// </summary>
	/// <returns>An awaitable Task that completes when the operation has completed.</returns>
	private async Task ApplyFiltersClickAsync() => await UpdateGridAsync();

	/// <summary>
	/// The click event handler for the reset filters button.
	/// </summary>
	/// <returns></returns>
	private async Task ResetFiltersClickAsync()
	{
		await ResetFiltersAndSortersAsync();
		await UpdateGridAsync();
	}

	/// <summary>
	/// The click event handler for a column heading used to trigger sorting of grid data.
	/// </summary>
	/// <param name="target">The column that has been clicked.</param>
	/// <returns>A <see cref="Task"/> that completes when the grid has been updated.</returns>
	private async Task ColumnHeadingClickAsync(IUmbrellaColumnDefinition<TItem> target)
	{
		foreach (var column in ColumnDefinitions)
		{
			column.Direction = column == target
				? column.Direction switch
				{
					SortDirection.Ascending => SortDirection.Descending,
					SortDirection.Descending => SortDirection.Ascending,
					_ => SortDirection.Ascending
				}
				: null;
		}

		await UpdateGridAsync();
	}

	/// <summary>
	/// The click event handler for the reload button.
	/// </summary>
	/// <returns>A <see cref="Task"/> that completes when the grid has been reloaded.</returns>
	private async Task ReloadButtonClickAsync()
	{
		await ResetFiltersAndSortersAsync();
		await UpdateGridAsync(PageNumber, PageSize);
	}

	private static async Task FilterTextAddonButtonClickAsync(IUmbrellaColumnDefinition<TItem> columnDefinition)
	{
		if (columnDefinition.OnAddOnButtonClickedAsync is null)
			throw new UmbrellaBlazorException($"The {nameof(columnDefinition.OnAddOnButtonClickedAsync)} has not been specified.");

		columnDefinition.FilterValue = await columnDefinition.OnAddOnButtonClickedAsync(columnDefinition.FilterValue);
	}

	/// <summary>
	/// The event handler invoked by the <see cref="UmbrellaPagination"/> component when its state has changed as result of user interaction.
	/// </summary>
	/// <param name="args">The event arguments containing the updated state of the pagination component.</param>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	private Task OnPaginationOptionsChangedAsync(UmbrellaPaginationEventArgs args) => UpdateGridAsync(args.PageNumber, args.PageSize);

	private async Task ResetFiltersAndSortersAsync()
	{
		foreach (var column in ColumnDefinitions)
		{
			column.FilterValue = null;
			column.Direction = null;

			if (column.PropertyName == InitialSortPropertyName)
			{
				column.Direction = InitialSortDirection;
			}

			// TODO: InitialFilters
		}

		if (OnResetFiltersAndSorters.HasDelegate)
			await OnResetFiltersAndSorters.InvokeAsync();
	}

	private async Task UpdateGridAsync(int? pageNumber = null, int? pageSize = null)
	{
		PageNumber = pageNumber ?? 1;

		if (pageSize.HasValue)
		{
			PageSize = pageSize.Value;
		}

		if (OnGridOptionsChanged.HasDelegate)
		{
			List<SortExpressionDescriptor>? lstSorters = null;
			List<FilterExpressionDescriptor>? lstFilters = null;

			foreach (var column in ColumnDefinitions)
			{
				if (string.IsNullOrEmpty(column.PropertyName) || column.DisplayMode == UmbrellaColumnDisplayMode.None)
				{
					continue;
				}

				if (column.Sortable && column.Direction.HasValue)
				{
					lstSorters ??= new List<SortExpressionDescriptor>();
					lstSorters.Add(new SortExpressionDescriptor(column.SorterMemberPathOverride ?? column.PropertyName, column.Direction.Value));
				}

				if (column.Filterable && !string.IsNullOrEmpty(column.FilterValue))
				{
					lstFilters ??= new List<FilterExpressionDescriptor>();

					string? filterValue = column.FilterOptionsType switch
					{
						UmbrellaColumnFilterOptionsType.String when column.FilterControlType is UmbrellaColumnFilterType.Options && column.FilterValue.Equals("any", StringComparison.OrdinalIgnoreCase) => null,
						UmbrellaColumnFilterOptionsType.String => column.FilterValue,
						UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("yes", StringComparison.OrdinalIgnoreCase) => "true",
						UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("no", StringComparison.OrdinalIgnoreCase) => "false",
						UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("any", StringComparison.OrdinalIgnoreCase) => null,
						UmbrellaColumnFilterOptionsType.Enum when column.FilterValue.Equals("any", StringComparison.OrdinalIgnoreCase) => null,
						_ => column.FilterValue
					};

					if (!string.IsNullOrWhiteSpace(filterValue))
					{
						if (column.FilterControlType is UmbrellaColumnFilterType.DateRange)
						{
							DateTimeRange dateRange = JsonSerializer.Deserialize(column.FilterValue, DateTimeRangeJsonSerializerContext.Default.DateTimeRange);

							if (dateRange.StartDate != DateTime.MinValue && dateRange.EndDate != DateTime.MaxValue)
							{
								lstFilters.Add(new FilterExpressionDescriptor(column.FilterMemberPathOverride ?? column.PropertyName, dateRange.StartDate.ToString("O") + "Z", FilterType.GreaterThanOrEqual));
								lstFilters.Add(new FilterExpressionDescriptor(column.FilterMemberPathOverride ?? column.PropertyName, dateRange.EndDate.ToString("O") + "Z", FilterType.LessThanOrEqual));
							}
						}
						else
						{
							// Treating explicit values as special cases here where we want to intentionally pass a null value, e.g. for a nullable enum
							// where we want to filter only on null.
							if (filterValue is "null")
							{
								filterValue = null;
							}
							else if (DateTime.TryParse(filterValue, out DateTime dtFilter))
							{
								filterValue = dtFilter.ToString("O") + "Z";
							}

							lstFilters.Add(new FilterExpressionDescriptor(column.FilterMemberPathOverride ?? column.PropertyName, filterValue, column.FilterMatchType));
						}
					}
				}
			}

			static IReadOnlyCollection<T> EnsureCollection<T>(List<T>? coll)
			{
				IReadOnlyCollection<T>? target = coll;

				target ??= Array.Empty<T>();

				return target;
			}

			await OnGridOptionsChanged.InvokeAsync(new UmbrellaGridRefreshEventArgs(PageNumber, PageSize, EnsureCollection(lstSorters), EnsureCollection(lstFilters)));
		}
	}

	private async Task DateRangeSelectionButtonClickAsync(IUmbrellaColumnDefinition<TItem> column)
	{
		try
		{
			var result = await DialogService.ShowDialogAsync<DateRangeDialog>("Select Dates", "");

			if (result.Cancelled)
				return;

			if (result.Data is not DateRangeDialogModel resultModel)
				throw new InvalidOperationException("There has been a problem determining the selected date range.");

			var updatedDateRange = new DateTimeRange
			{
				StartDate = resultModel.StartDate ?? DateTime.MinValue,
				EndDate = resultModel.EndDate ?? DateTime.MinValue
			};

			column.FilterValue = JsonSerializer.Serialize(updatedDateRange, typeof(DateTimeRange), DateTimeRangeJsonSerializerContext.Default);
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			await DialogService.ShowDangerMessageAsync();
		}
	}

	private async ValueTask ApplyQueryStringSortersAndFiltersAsync()
	{
		bool updateGrid = false;

		var sortByResult = Navigation.TryGetQueryStringValue<string>("sortBy");
		var sortDirectionResult = Navigation.TryGetQueryStringEnumValue<SortDirection>("sortDirection");
		var filtersResult = Navigation.TryGetQueryStringValue<string>("filters");

		if (Logger.IsEnabled(LogLevel.Debug))
			Logger.WriteDebug(new { sortByResult, sortDirectionResult, filtersResult });

		if (sortByResult.success)
		{
			IUmbrellaColumnDefinition<TItem>? sortColumn = ColumnDefinitions.FirstOrDefault(x => x.PropertyName?.Equals(sortByResult.value, StringComparison.OrdinalIgnoreCase) is true && x.Sortable);

			if (sortColumn is not null)
			{
				sortColumn.Direction = sortDirectionResult.success ? sortDirectionResult.value : SortDirection.Ascending;
				updateGrid = true;

				if (Logger.IsEnabled(LogLevel.Debug))
					Logger.WriteDebug(new { sortColumn.PropertyName, sortColumn.Direction }, "Applied Sorter");
			}
		}

		if (filtersResult.success)
		{
			Dictionary<string, string>? dicFilters = JsonSerializer.Deserialize<Dictionary<string, string>>(filtersResult.value, _jsonSerializerOptions);

			if (dicFilters is { Count: > 0 })
			{
				foreach (var kvp in dicFilters)
				{
					IUmbrellaColumnDefinition<TItem>? filterableColumn = FilterableColumns?.FirstOrDefault(x => x.PropertyName?.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase) is true);

					if (filterableColumn is not null)
					{
						filterableColumn.FilterValue = kvp.Value;
						updateGrid = true;

						if (Logger.IsEnabled(LogLevel.Debug))
							Logger.WriteDebug(new { filterableColumn.PropertyName, filterableColumn.FilterValue }, "Applied Filter");
					}
				}
			}
		}

		if (updateGrid)
			await UpdateGridAsync();
	}
}