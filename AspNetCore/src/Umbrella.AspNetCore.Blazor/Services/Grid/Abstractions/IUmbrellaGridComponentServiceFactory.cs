using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Services.Abstractions;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Primitives.Abstractions;

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
	/// <param name="loadPaginatedResultModelDelegate">The delegate used to load the paginated results.</param>
	/// <param name="stateHasChangedDelegate">The StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.</param>
	/// <param name="callGridStateHasChangedOnRefresh">Gets a value specifying whether or not the <c>GridInstance</c> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <c>UpdateAsync</c> method called internally
	/// by the <c>RefreshGridAsync</c> method.</param>
	/// <param name="initialFilterExpressions">The initial filter expressions.</param>
	/// <returns>The created instance.</returns>
	/// <exception cref="UmbrellaBlazorException">There has been a problem creating the service.</exception>
	IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel> CreateUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>(Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, CancellationToken, Task<IOperationResult<TPaginatedResultModel?>>> loadPaginatedResultModelDelegate, Action stateHasChangedDelegate, bool callGridStateHasChangedOnRefresh = true, IReadOnlyCollection<FilterExpressionDescriptor>? initialFilterExpressions = null)
		where TItemModel : notnull
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>;

	/// <summary>
	/// Creates an instance of the <see cref="UmbrellaReadOnlyRemoteDataAccessGridComponentService{TItemModel, TPaginatedResultModel, TRepository}"/>.
	/// </summary>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <param name="stateHasChangedDelegate">The StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.</param>
	/// <param name="loadPaginatedResultModelDelegate">The delegate used to load the paginated results.</param>
	/// <param name="callGridStateHasChangedOnRefresh">Gets a value specifying whether or not the <c>GridInstance</c> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <c>UpdateAsync</c> method called internally
	/// by the <c>RefreshGridAsync</c> method.</param>
	/// <param name="initialFilterExpressions">The initial filter expressions.</param>
	/// <returns>The created instance.</returns>
	/// <exception cref="UmbrellaBlazorException">There has been a problem creating the service.</exception>
	IUmbrellaReadOnlyRemoteDataAccessGridComponentService<TItemModel, TPaginatedResultModel, TRepository> CreateUmbrellaReadOnlyRemoteDataAccessGridComponentService<TItemModel, TPaginatedResultModel, TRepository>(Action stateHasChangedDelegate, Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, CancellationToken, Task<IOperationResult<TPaginatedResultModel?>>>? loadPaginatedResultModelDelegate = null, bool callGridStateHasChangedOnRefresh = true, IReadOnlyCollection<FilterExpressionDescriptor>? initialFilterExpressions = null)
		where TItemModel : class
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>
		where TRepository : class, IReadOnlyPaginatedSlimItemGenericDataRepository<TItemModel, TPaginatedResultModel>;

	/// <summary>
	/// Creates an instance of the <see cref="UmbrellaRemoteDataAccessGridComponentService{TItemModel, TIdentifier, TPaginatedResultModel, TRepository}"/>.
	/// </summary>
	/// <typeparam name="TItemModel">The type of the item model.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <typeparam name="TRepository">The type of the repository.</typeparam>
	/// <param name="stateHasChangedDelegate">The StateHasChanged delegate. This should always be initialized to <see cref="ComponentBase.StateHasChanged"/>.</param>
	/// <param name="loadPaginatedResultModelDelegate">The delegate used to load the paginated results.</param>
	/// <param name="callGridStateHasChangedOnRefresh">Gets a value specifying whether or not the <c>GridInstance</c> should internally call its <see cref="ComponentBase.StateHasChanged"/> method
	/// when this component invokes its <c>UpdateAsync</c> method called internally
	/// by the <c>RefreshGridAsync</c> method.</param>
	/// <param name="initialFilterExpressions">The initial filter expressions.</param>
	/// <param name="beforeDeletingDelegate">
	/// An optional delegate that is invoked after showing the confirmation dialog asking the user to confirm deletion,
	/// but before the calling the <c>DeleteAsync</c> method on the repository.
	/// </param>
	/// <returns>The created instance.</returns>
	/// <exception cref="UmbrellaBlazorException">There has been a problem creating the service.</exception>
	IUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> CreateUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository>(Action stateHasChangedDelegate, Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, CancellationToken, Task<IOperationResult<TPaginatedResultModel?>>>? loadPaginatedResultModelDelegate = null, bool callGridStateHasChangedOnRefresh = true, IReadOnlyCollection<FilterExpressionDescriptor>? initialFilterExpressions = null, Func<TItemModel, ValueTask<bool>>? beforeDeletingDelegate = null)
		where TItemModel : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>
		where TRepository : class, IReadOnlyPaginatedSlimItemGenericDataRepository<TItemModel, TPaginatedResultModel>, IDeleteItemGenericDataService<TIdentifier>;
}