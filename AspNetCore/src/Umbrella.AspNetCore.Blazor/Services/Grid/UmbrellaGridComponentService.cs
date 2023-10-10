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
	/// <param name="dialogService">The dialog utility.</param>
	internal UmbrellaGridComponentService(
		ILogger<UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>> logger,
		IUmbrellaDialogService dialogService)
	{
		Logger = logger;
		DialogService = dialogService;
	}

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the dialog utility.
	/// </summary>
	protected IUmbrellaDialogService DialogService { get; }

	/// <inheritdoc />
	public IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; internal set; } = Array.Empty<FilterExpressionDescriptor>();

	/// <inheritdoc />
	public UmbrellaGrid<TItemModel> GridInstance { get; set; } = null!;

	/// <inheritdoc />
	public bool CallGridStateHasChangedOnRefresh { get; init; } = true;

	/// <inheritdoc />
	public required Action StateHasChangedDelegate { get; init; }

	/// <inheritdoc />
	public Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, CancellationToken, Task<IHttpCallResult<TPaginatedResultModel?>>> LoadPaginatedResultModelDelegate { get; internal set; } = null!;

	/// <summary>
	/// Invoked by the <see cref="GridInstance"/> when it requests new data.
	/// </summary>
	/// <param name="args">The request containing details of the current filtering, sorting and pagination options.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The response provided to the grid.</returns>
	public async Task<UmbrellaGridDataResponse<TItemModel>?> OnGridDataRequestAsync(UmbrellaGridDataRequest args, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await LoadPaginatedResultModelDelegate(args.PageNumber, args.PageSize, args.Sorters, args.Filters, cancellationToken);

			if (result.Success && result.Result is not null)
			{
				if (!CallGridStateHasChangedOnRefresh)
					StateHasChangedDelegate();

				return new UmbrellaGridDataResponse<TItemModel>(result.Result.Items, result.Result.TotalCount, result.Result.PageNumber, result.Result.PageSize, CallGridStateHasChangedOnRefresh);
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
			await DialogService.ShowDangerMessageAsync();
		}

		return null;
	}

	/// <summary>
	/// Refreshes the grid, retaining its state.
	/// </summary>
	/// <returns>An awaitable Task that completes when this operation has completed.</returns>
	protected Task RefreshGridAsync() => GridInstance.RefreshAsync();

	/// <summary>
	/// Shows the problem details error message. If this does not exist, the error message defaults to <see cref="DialogDefaults.UnknownErrorMessage"/>.
	/// </summary>
	/// <param name="problemDetails">The problem details.</param>
	/// <param name="title">The title.</param>
	protected ValueTask ShowProblemDetailsErrorMessageAsync(HttpProblemDetails? problemDetails, string title = "Error")
		=> DialogService.ShowProblemDetailsErrorMessageAsync(problemDetails, title);
}