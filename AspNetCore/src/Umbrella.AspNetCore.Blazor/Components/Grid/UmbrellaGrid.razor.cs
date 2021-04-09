using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.Pagination;
using Umbrella.AspNetCore.Blazor.Enumerations;
using Umbrella.AspNetCore.Blazor.Utilities.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	// TODO: Maybe it's better to create a new CollectionView control here. This Grid adds nothing more than a loop and some basic structure for that scenario.
	// Solve the double request bug first and then do that. Could even have some kind of base component that encapsulates core functionality, e.g. auto-scroll.
	// TODO: Rename to Table and CollectionView
	// TODO: Create a PaginationMode option to replace ShowPagination: None, Top, Bottom, Both. Do the same for caption. Default to Bottom.
	public enum UmbrellaGridRenderMode
	{
		Full,
		Slim
	}

	public partial class UmbrellaGrid<TItem> : IUmbrellaGrid
	{
		private bool _autoScrollEnabled;

		[Inject]
		private IUmbrellaBlazorInteropUtility BlazorInteropUtility { get; set; } = null!;

		/// <summary>
		/// Gets or sets the instance of the associated <see cref="UmbrellaPagination"/> component.
		/// </summary>
		protected UmbrellaPagination PaginationInstance { get; set; } = null!;

		protected bool ColumnScanComplete { get; set; }

		protected List<UmbrellaColumnDefinition> Columns { get; } = new List<UmbrellaColumnDefinition>();

		/// <inheritdoc />
		public string? FirstColumnPropertyName { get; private set; }

		protected IReadOnlyCollection<UmbrellaColumnDefinition>? FilterableColumns { get; private set; }

		/// <summary>
		/// Gets the current <see cref="LayoutState"/> of the component.
		/// </summary>
		public LayoutState CurrentState { get; private set; } = LayoutState.Loading;
		protected int TotalCount { get; set; }
		public int PageNumber { get; private set; } = UmbrellaPaginationDefaults.PageNumber;
		public int PageSize { get; private set; } = UmbrellaPaginationDefaults.PageSize;

		[Parameter]
		public RenderFragment<TItem>? ChildContent { get; set; }

		public IReadOnlyCollection<TItem> Items { get; set; } = Array.Empty<TItem>();

		[Parameter]
		public string LoadingMessage { get; set; } = "Loading... Please wait.";

		[Parameter]
		public string EmptyMessage { get; set; } = "There is either no data to display or your search options have no results.";

		[Parameter]
		public string ErrorMessage { get; set; } = "There has been a problem. Please try again.";

		[Parameter]
		public bool ShowReloadButton { get; set; }

		[Parameter]
		public string FilterOptionsHeading { get; set; } = "Search Options";

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
		public UmbrellaGridRenderMode RenderMode { get; set; } = UmbrellaGridRenderMode.Full;

		[Parameter]
		public string InitialSortPropertyName { get; set; } = null!;

		[Parameter]
		public SortDirection InitialSortDirection { get; set; } = SortDirection.Descending;

		[Parameter]
		public EventCallback<UmbrellaGridRefreshEventArgs> OnGridOptionsChanged { get; set; }

		[Parameter]
		public IReadOnlyCollection<int> PageSizeOptions { get; set; } = UmbrellaPaginationDefaults.PageSizeOptions;

		[Parameter]
		public string? GridCssClass { get; set; } = "table table-hover table-sm table-responsive-sm";

		[Parameter]
		public string? ItemCssClass { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the pagination controls should be rendered using smaller styling.
		/// </summary>
		[Parameter]
		public bool SmallPagination { get; set; } = true;

		[Parameter]
		public bool AutoScrollTop { get; set; } = true;

		[Parameter]
		public int ScrollTopOffset { get; set; }

		/// <summary>
		/// Gets the caption text.
		/// </summary>
		protected string CaptionText
		{
			get
			{
				int startItem = (PageNumber - 1) * PageSize + 1;
				int endItem = Math.Min(PageSize, Items.Count) + (PageNumber == 1 ? 0 : startItem - 1);

				if (TotalCount == 1)
					return "Showing 1 of 1 items";

				return $"Showing items {startItem} to {endItem} of {TotalCount}";
			}
		}

		/// <inheritdoc />
		protected override void OnParametersSet() => Guard.ArgumentNotNullOrWhiteSpace(InitialSortPropertyName, nameof(InitialSortPropertyName));

		/// <inheritdoc />
		public void AddColumnDefinition(UmbrellaColumnDefinition column) => Columns.Add(column);

		/// <inheritdoc />
		public void SetColumnScanCompleted()
		{
			if (!ColumnScanComplete)
			{
				ColumnScanComplete = true;

				var filterableColumns = new List<UmbrellaColumnDefinition>();

				for (int i = 0; i < Columns.Count; i++)
				{
					var column = Columns[i];

					if (i == 0)
						FirstColumnPropertyName = column.PropertyName;

					if (column.Filterable)
						filterableColumns.Add(column);

					if (InitialSortPropertyName == column.PropertyName)
						column.Direction = InitialSortDirection;
				}

				FilterableColumns = filterableColumns;

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

		public void Update(IReadOnlyCollection<TItem> items, int? totalCount = null, int? pageNumber = null, int? pageSize = null)
		{
			Items = items;
			TotalCount = totalCount ?? TotalCount;
			PageSize = pageSize ?? PageSize;
			PageNumber = pageNumber ?? PageNumber;

			if (AutoScrollTop && _autoScrollEnabled)
				BlazorInteropUtility.AnimateScrollToAsync(".u-grid", ScrollTopOffset);

			// Only enable auto-scrolling after the initial page load.
			_autoScrollEnabled = true;

			CurrentState = Items.Count > 0 ? LayoutState.Success : LayoutState.Empty;

			StateHasChanged();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (!firstRender && PaginationInstance != null)
				await PaginationInstance.UpdateAsync(TotalCount, PageNumber, PageSize);
		}

		protected async Task ApplyFiltersClick() => await UpdateGridAsync();

		protected async Task ResetFiltersClick()
		{
			ResetFiltersAndSorters();
			await UpdateGridAsync();
		}

		protected async Task ColumnHeadingClick(UmbrellaColumnDefinition target)
		{
			foreach (var column in Columns)
			{
				if (column == target)
				{
					column.Direction = column.Direction switch
					{
						SortDirection.Ascending => SortDirection.Descending,
						SortDirection.Descending => SortDirection.Ascending,
						_ => SortDirection.Ascending
					};
				}
				else
				{
					column.Direction = null;
				}
			}

			await UpdateGridAsync();
		}

		protected async Task ReloadButtonClick()
		{
			ResetFiltersAndSorters();
			await UpdateGridAsync(PageNumber, PageSize);
		}

		protected Task OnPaginationOptionsChangedAsync(UmbrellaPaginationEventArgs args) => UpdateGridAsync(args.PageNumber, args.PageSize);

		private void ResetFiltersAndSorters()
		{
			foreach (var column in Columns)
			{
				column.FilterValue = null;
				column.Direction = null;

				if (column.PropertyName == InitialSortPropertyName)
					column.Direction = InitialSortDirection;
			}
		}

		private async Task UpdateGridAsync(int? pageNumber = null, int? pageSize = null)
		{
			PageNumber = pageNumber ?? 1;

			if (pageSize.HasValue)
				PageSize = pageSize.Value;

			if (OnGridOptionsChanged.HasDelegate)
			{
				List<SortExpressionDescriptor>? lstSorters = null;
				List<FilterExpressionDescriptor>? lstFilters = null;

				foreach (var column in Columns)
				{
					if (string.IsNullOrWhiteSpace(column.PropertyName))
						continue;

					if (column.Sortable && column.Direction.HasValue)
					{
						lstSorters ??= new List<SortExpressionDescriptor>();
						lstSorters.Add(new SortExpressionDescriptor(column.PropertyName, column.Direction.Value));
					}

					if (column.Filterable && !string.IsNullOrWhiteSpace(column.FilterValue))
					{
						lstFilters ??= new List<FilterExpressionDescriptor>();

						string? filterValue = column.FilterOptionsType switch
						{
							UmbrellaColumnFilterOptionsType.String => column.FilterValue,
							UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("yes", StringComparison.OrdinalIgnoreCase) => "true",
							UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("no", StringComparison.OrdinalIgnoreCase) => "false",
							UmbrellaColumnFilterOptionsType.Boolean when column.FilterValue.Equals("any", StringComparison.OrdinalIgnoreCase) => null,
							UmbrellaColumnFilterOptionsType.Enum when column.FilterValue.Equals("any", StringComparison.OrdinalIgnoreCase) => null,
							_ => column.FilterValue
						};

						if (!string.IsNullOrWhiteSpace(filterValue))
							lstFilters.Add(new FilterExpressionDescriptor(column.PropertyName, filterValue, column.FilterMatchType));
					}
				}

				static IReadOnlyCollection<T> EnsureCollection<T>(List<T>? coll)
				{
					IReadOnlyCollection<T>? target = coll;

					if (target is null)
						target = Array.Empty<T>();

					return target;
				}

				await OnGridOptionsChanged.InvokeAsync(new UmbrellaGridRefreshEventArgs(PageNumber, PageSize, EnsureCollection(lstSorters), EnsureCollection(lstFilters)));
			}
		}
	}
}