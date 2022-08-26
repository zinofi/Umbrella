// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with Blazor components that contain a single <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
/// <seealso cref="UmbrellaComponentBase" />
public abstract class UmbrellaGridComponentBase<TItemModel, TPaginatedResultModel> : UmbrellaComponentBase
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
{
	/// <summary>
	/// Gets the property name used to initally sort the data in the grid.
	/// </summary>
	protected abstract string InitialSortPropertyName { get; }

	/// <summary>
	/// Gets the sort direction used to initially sort the data in the grid.
	/// </summary>
	protected abstract SortDirection InitialSortDirection { get; }

	/// <summary>
	/// Gets the collection of sort expressions used to initially sort the data in the grid.
	/// </summary>
	/// <remarks>
	/// Defaults to a collection containing a single sort expresssion which uses the <see cref="InitialSortPropertyName"/> and <see cref="InitialSortDirection"/>.
	/// </remarks>
	protected virtual Lazy<IReadOnlyCollection<SortExpressionDescriptor>> InitialSortExpressions { get; }

	/// <summary>
	/// Gets or sets the current refresh options which are updated when the grid options have been changed as a result of filtering, sorting or paginating through data.
	/// </summary>
	/// <remarks>
	/// This is updated internally inside this component by the <see cref="OnGridOptionsChangedAsync(UmbrellaGridRefreshEventArgs)"/> event handler.
	/// </remarks>
	protected UmbrellaGridRefreshEventArgs? CurrentRefreshOptions { get; set; }

	/// <summary>
	/// Gets or sets the grid instance.
	/// </summary>
	protected UmbrellaGrid<TItemModel> GridInstance { get; set; } = null!;

	/// <summary>
	/// Gets a value specifying whether or not the <see cref="GridInstance"/> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <see cref="UmbrellaGrid{TItem}.UpdateAsync(IReadOnlyCollection{TItem}, int?, int?, int?, bool)"/> method called internally
	/// by the <see cref="RefreshGridAsync(int, int, IEnumerable{SortExpressionDescriptor}?, IEnumerable{FilterExpressionDescriptor}?)"/> method.
	/// </summary>
	/// <remarks>Defaults to <see langword="true"/>.</remarks>
	protected virtual bool CallGridStateHasChangedOnRefresh { get; } = true;

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaGridComponentBase{TItemModel, TPaginatedResultModel}"/> class.
	/// </summary>
	public UmbrellaGridComponentBase()
	{
		InitialSortExpressions = new Lazy<IReadOnlyCollection<SortExpressionDescriptor>>(() => new[] { new SortExpressionDescriptor(InitialSortPropertyName, InitialSortDirection) });
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		try
		{
			if (!firstRender)
				return;

			await RefreshGridAsync(GridInstance.PageNumber, GridInstance.PageSize, sorters: InitialSortExpressions.Value);
		}
		catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}

	/// <summary>
	/// The event handler invoked by the <see cref="GridInstance"/> as a result of filtering, sorting or pagination.
	/// </summary>
	/// <param name="args">The event args containing details of the current filtering, sorting and pagination options.</param>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	protected async Task OnGridOptionsChangedAsync(UmbrellaGridRefreshEventArgs args)
	{
		try
		{
			CurrentRefreshOptions = args;

			await RefreshGridAsync(args.PageNumber, args.PageSize, args.Sorters, args.Filters);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { args }, returnValue: true))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}

	/// <summary>
	/// Loads new data from the server using the specified options.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <returns>The results from the server.</returns>
	protected abstract Task<IHttpCallResult<TPaginatedResultModel?>> LoadPaginatedResultModelAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null);

	/// <summary>
	/// Refreshes the data in the grid using the specified options. Internally this calls <see cref="LoadPaginatedResultModelAsync(int, int, IEnumerable{SortExpressionDescriptor}?, IEnumerable{FilterExpressionDescriptor}?)" />
	/// to load new data, usually from the server.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	protected virtual async Task RefreshGridAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null)
	{
		try
		{
			var result = await LoadPaginatedResultModelAsync(pageNumber, pageSize, sorters, filters);

			if (result.Success && result.Result != null)
			{
				await GridInstance.UpdateAsync(result.Result.Items, result.Result.TotalCount, result.Result.PageNumber, result.Result.PageSize, CallGridStateHasChangedOnRefresh);

				if (!CallGridStateHasChangedOnRefresh)
					StateHasChanged();
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
}