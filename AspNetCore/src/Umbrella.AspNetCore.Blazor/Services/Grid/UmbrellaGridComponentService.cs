using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Services.Constants;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services.Grid;

/// <summary>
/// A service that can be used with Blazor components that contain a <see cref="UmbrellaGrid{TItem}"/> component.
/// Multiple instances of this service can be used to power multiple grids contained within a single Blazor component.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
public class UmbrellaGridComponentService<TItemModel, TPaginatedResultModel> : IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>
	where TItemModel : notnull
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaGridComponentService{TItemModel, TPaginatedResultModel}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dialogUtility">The dialog utility.</param>
	internal UmbrellaGridComponentService(
		ILogger<UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>> logger,
		IUmbrellaDialogService dialogUtility)
	{
		Logger = logger;
		DialogUtility = dialogUtility;
		InitialSortExpressions = new Lazy<IReadOnlyCollection<SortExpressionDescriptor>>(() => new[] { new SortExpressionDescriptor(InitialSortPropertyName, InitialSortDirection) });
	}

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the dialog utility.
	/// </summary>
	protected IUmbrellaDialogService DialogUtility { get; }

	/// <inheritdoc />
	public required string InitialSortPropertyName { get; init; } = null!;

	/// <inheritdoc />
	public SortDirection InitialSortDirection { get; init; } = SortDirection.Descending;

	/// <inheritdoc />
	public Lazy<IReadOnlyCollection<SortExpressionDescriptor>> InitialSortExpressions { get; internal set; }

	/// <inheritdoc />
	public IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; internal set; } = Array.Empty<FilterExpressionDescriptor>();

	/// <inheritdoc />
	public UmbrellaGridRefreshEventArgs? CurrentRefreshOptions { get; protected set; }

	/// <inheritdoc />
	public UmbrellaGrid<TItemModel> GridInstance { get; set; } = null!;

	/// <inheritdoc />
	public bool CallGridStateHasChangedOnRefresh { get; init; } = true;

	/// <inheritdoc />
	public bool AutoRenderOnPageLoad { get; init; } = true;

	/// <inheritdoc />
	public required Action StateHasChangedDelegate { get; init; }

	/// <inheritdoc />
	public Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>> LoadPaginatedResultModelDelegate { get; internal set; } = null!;

	/// <inheritdoc />
	public async Task InitializeGridAsync()
	{
		try
		{
			if (AutoRenderOnPageLoad)
				throw new InvalidOperationException("Auto rendering has been enabled. This method should not be manually called.");

			await RefreshGridAsync(GridInstance.PageNumber, GridInstance.PageSize, sorters: InitialSortExpressions.Value, filters: InitialFilterExpressions);
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}

	/// <inheritdoc />
	public async Task OnAfterRenderAsync(bool firstRender)
	{
		try
		{
			if (!firstRender || !AutoRenderOnPageLoad)
				return;

			await RefreshGridAsync(GridInstance.PageNumber, GridInstance.PageSize, sorters: InitialSortExpressions.Value, filters: InitialFilterExpressions);
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}

	/// <inheritdoc />
	public async Task OnGridOptionsChangedAsync(UmbrellaGridRefreshEventArgs args)
	{
		try
		{
			CurrentRefreshOptions = args;

			await RefreshGridAsync(args.PageNumber, args.PageSize, args.Sorters, args.Filters);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { args }))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}

	/// <inheritdoc />
	public async Task RefreshGridAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null)
	{
		try
		{
			var result = await LoadPaginatedResultModelDelegate(pageNumber, pageSize, sorters, filters);

			if (result.Success && result.Result is not null)
			{
				await GridInstance.UpdateAsync(result.Result.Items, result.Result.TotalCount, result.Result.PageNumber, result.Result.PageSize, CallGridStateHasChangedOnRefresh);

				if (!CallGridStateHasChangedOnRefresh)
					StateHasChangedDelegate();
			}
			else
			{
				GridInstance.SetErrorState();
				await ShowProblemDetailsErrorMessageAsync(result.ProblemDetails);
			}
		}
		catch
		{
			GridInstance.SetErrorState();
			throw;
		}
	}

	/// <inheritdoc />
	public Task RefreshGridAsyncUsingCurrentRefreshOptionsAsync()
		=> RefreshGridAsync(CurrentRefreshOptions?.PageNumber ?? GridInstance.PageNumber,
			CurrentRefreshOptions?.PageSize ?? GridInstance.PageSize,
			CurrentRefreshOptions?.Sorters ?? InitialSortExpressions.Value,
			CurrentRefreshOptions?.Filters ?? InitialFilterExpressions);

	/// <summary>
	/// Shows the problem details error message. If this does not exist, the error message defaults to <see cref="DialogDefaults.UnknownErrorMessage"/>.
	/// </summary>
	/// <param name="problemDetails">The problem details.</param>
	/// <param name="title">The title.</param>
	protected ValueTask ShowProblemDetailsErrorMessageAsync(HttpProblemDetails? problemDetails, string title = "Error")
		=> DialogUtility.ShowDangerMessageAsync(problemDetails?.Detail ?? DialogDefaults.UnknownErrorMessage, title);
}