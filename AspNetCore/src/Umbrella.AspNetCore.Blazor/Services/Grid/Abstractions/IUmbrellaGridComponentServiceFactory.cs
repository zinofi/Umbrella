// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;

/// <summary>
/// A factory for creating instances of <see cref="UmbrellaGridComponentService{TItemModel, TPaginatedResultModel}"/>
/// and <see cref="UmbrellaRemoteDataAccessGridComponentService{TItemModel, TIdentifier, TPaginatedResultModel, TRepository}"/>.
/// </summary>
public interface IUmbrellaGridComponentServiceFactory
{
	/// <summary>
	/// Creates an instance of the <see cref="UmbrellaGridComponentService{TItemModel, TPaginatedResultModel}"/>.
	/// </summary>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <param name="initialSortPropertyName">The sort direction used to initially sort the data in the grid.</param>
	/// <param name="loadPaginatedResultModelDelegate">The delegate used to load the paginated results.</param>
	/// <param name="StateHasChangedDelegate">The StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.</param>
	/// <param name="autoRenderOnPageLoad">Specifies whether or not the grid will automatically render when the page loads. If this is set to <see langword="false" />
	/// the <c>InitializeGridAsync</c> method should be manually called.</param>
	/// <param name="callGridStateHasChangedOnRefresh">Gets a value specifying whether or not the <c>GridInstance</c> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <c>UpdateAsync</c> method called internally
	/// by the <c>RefreshGridAsync</c> method.</param>
	/// <param name="initialSortDirection">The sort direction used to initially sort the data in the grid.</param>
	/// <param name="initialSortExpressions">
	/// The collection of sort expressions used to initially sort the data in the grid.
	/// Defaults to a collection containing a single sort expresssion which uses the <paramref name="initialSortPropertyName"/> and <paramref name="initialSortDirection"/>.
	/// </param>
	/// <returns>The created instance.</returns>
	/// <exception cref="UmbrellaBlazorException">There has been a problem creating the service.</exception>
	IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel> CreateUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>(string initialSortPropertyName, Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>> loadPaginatedResultModelDelegate, Action StateHasChangedDelegate, bool autoRenderOnPageLoad = true, bool callGridStateHasChangedOnRefresh = true, SortDirection initialSortDirection = SortDirection.Descending, Lazy<IReadOnlyCollection<SortExpressionDescriptor>>? initialSortExpressions = null) where TPaginatedResultModel : PaginatedResultModel<TItemModel>;

	/// <summary>
	/// Creates an instance of the <see cref="UmbrellaRemoteDataAccessGridComponentService{TItemModel, TIdentifier, TPaginatedResultModel, TRepository}"/>.
	/// </summary>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <param name="initialSortPropertyName">The sort direction used to initially sort the data in the grid.</param>
	/// <param name="loadPaginatedResultModelDelegate">The delegate used to load the paginated results.</param>
	/// <param name="StateHasChangedDelegate">The StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.</param>
	/// <param name="autoRenderOnPageLoad">Specifies whether or not the grid will automatically render when the page loads. If this is set to <see langword="false" />
	/// the <c>InitializeGridAsync</c> method should be manually called.</param>
	/// <param name="callGridStateHasChangedOnRefresh">Gets a value specifying whether or not the <c>GridInstance</c> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <c>UpdateAsync</c> method called internally
	/// by the <c>RefreshGridAsync</c> method.</param>
	/// <param name="initialSortDirection">The sort direction used to initially sort the data in the grid.</param>
	/// <param name="initialSortExpressions">
	/// The collection of sort expressions used to initially sort the data in the grid.
	/// Defaults to a collection containing a single sort expresssion which uses the <paramref name="initialSortPropertyName"/> and <paramref name="initialSortDirection"/>.
	/// </param>
	/// <returns>The created instance.</returns>
	/// <exception cref="UmbrellaBlazorException">There has been a problem creating the service.</exception>
	IUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> CreateUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository>(string initialSortPropertyName, Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>> loadPaginatedResultModelDelegate, Action StateHasChangedDelegate, bool autoRenderOnPageLoad = true, bool callGridStateHasChangedOnRefresh = true, SortDirection initialSortDirection = SortDirection.Descending, Lazy<IReadOnlyCollection<SortExpressionDescriptor>>? initialSortExpressions = null)
		where TItemModel : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>
		where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>;
}