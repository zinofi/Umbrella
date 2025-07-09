using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with Blazor components that contain a single <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
/// <seealso cref="UmbrellaClientComponentBase" />
public abstract class UmbrellaGridComponentBase<TItemModel, TPaginatedResultModel> : UmbrellaClientComponentBase
	where TItemModel : notnull
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
{
	/// <summary>
	/// Gets the collection of filter expressions used to initially filter the data in the grid.
	/// </summary>
	/// <remarks>
	/// Defaults to an empty array.
	/// </remarks>
	protected virtual IReadOnlyCollection<FilterExpressionDescriptor> InitialFilterExpressions { get; } = Array.Empty<FilterExpressionDescriptor>();

	/// <summary>
	/// Gets or sets the grid instance.
	/// </summary>
	protected UmbrellaGrid<TItemModel> GridInstance { get; set; } = null!;

	/// <summary>
	/// Gets a value specifying whether or not the <see cref="GridInstance"/> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component responds to an <see cref="UmbrellaGridDataRequest"/> and provides a <see cref="UmbrellaGridDataResponse{TItem}"/>.
	/// </summary>
	/// <remarks>Defaults to <see langword="true"/>.</remarks>
	protected virtual bool CallGridStateHasChangedOnRefresh { get; } = true;

	/// <summary>
	/// Invoked by the <see cref="GridInstance"/> when it requests new data.
	/// </summary>
	/// <param name="args">The request containing details of the current filtering, sorting and pagination options.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The response provided to the grid.</returns>
	protected async Task<UmbrellaGridDataResponse<TItemModel>?> OnGridDataRequestAsync(UmbrellaGridDataRequest args, CancellationToken cancellationToken)
	{
		try
		{
			var result = await LoadPaginatedResultModelAsync(args.PageNumber, args.PageSize, args.Sorters, args.Filters, cancellationToken);

			if (result.Success && result.Result is not null)
			{
				if (!CallGridStateHasChangedOnRefresh)
					StateHasChanged();

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
			await DialogUtility.ShowDangerMessageAsync();
		}

		return null;
	}

	/// <summary>
	/// Loads new data from the server using the specified options.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The results from the server.</returns>
	protected abstract Task<IHttpCallResult<TPaginatedResultModel?>> LoadPaginatedResultModelAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null, CancellationToken cancellationToken = default);

	/// <inheritdoc />
	protected override async Task ReloadAsync()
	{
		await base.OnInitializedAsync();
		await GridInstance.RefreshAsync();
	}
}