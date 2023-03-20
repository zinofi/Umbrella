using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services.Grid;

/// <summary>
/// A factory for creating instances of <see cref="UmbrellaGridComponentService{TItemModel, TPaginatedResultModel}"/>
/// and <see cref="UmbrellaRemoteDataAccessGridComponentService{TItemModel, TIdentifier, TPaginatedResultModel, TRepository}"/>.
/// </summary>
public class UmbrellaGridComponentServiceFactory : IUmbrellaGridComponentServiceFactory
{
	private readonly ILogger _logger;
	private readonly ILoggerFactory _loggerFactory;
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaGridComponentServiceFactory"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="serviceProvider">The service provider.</param>
	public UmbrellaGridComponentServiceFactory(
		ILoggerFactory loggerFactory,
		IServiceProvider serviceProvider)
	{
		_logger = loggerFactory.CreateLogger<UmbrellaGridComponentServiceFactory>();
		_loggerFactory = loggerFactory;
		_serviceProvider = serviceProvider;
	}

	/// <inheritdoc/>
	public IUmbrellaGridComponentService<TItemModel, TPaginatedResultModel> CreateUmbrellaGridComponentService<TItemModel, TPaginatedResultModel>(
		string initialSortPropertyName,
		Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>> loadPaginatedResultModelDelegate,
		Action stateHasChangedDelegate,
		bool autoRenderOnPageLoad = true,
		bool callGridStateHasChangedOnRefresh = true,
		SortDirection initialSortDirection = SortDirection.Descending,
		Lazy<IReadOnlyCollection<SortExpressionDescriptor>>? initialSortExpressions = null)
		where TItemModel : notnull
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>
	{
		try
		{
			var logger = _loggerFactory.CreateLogger<UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>>();
			var dialogUtility = _serviceProvider.GetRequiredService<IUmbrellaDialogService>();

			var service = new UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>(logger, dialogUtility)
			{
				AutoRenderOnPageLoad = autoRenderOnPageLoad,
				CallGridStateHasChangedOnRefresh = callGridStateHasChangedOnRefresh,
				InitialSortDirection = initialSortDirection,
				InitialSortPropertyName = initialSortPropertyName,
				LoadPaginatedResultModelDelegate = loadPaginatedResultModelDelegate,
				StateHasChangedDelegate = stateHasChangedDelegate
			};

			if (initialSortExpressions is not null)
				service.InitialSortExpressions = initialSortExpressions;

			return service;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { initialSortPropertyName, autoRenderOnPageLoad, callGridStateHasChangedOnRefresh, initialSortDirection }))
		{
			throw new UmbrellaBlazorException("There has been a problem creating the service.", exc);
		}
	}

	/// <inheritdoc/>
	public IUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> CreateUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository>(
		string initialSortPropertyName,
		Action stateHasChangedDelegate,
		Func<int, int, IEnumerable<SortExpressionDescriptor>?, IEnumerable<FilterExpressionDescriptor>?, Task<IHttpCallResult<TPaginatedResultModel?>>>? loadPaginatedResultModelDelegate = null,
		bool autoRenderOnPageLoad = true,
		bool callGridStateHasChangedOnRefresh = true,
		SortDirection initialSortDirection = SortDirection.Descending,
		Lazy<IReadOnlyCollection<SortExpressionDescriptor>>? initialSortExpressions = null)
		where TItemModel : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
		where TPaginatedResultModel : PaginatedResultModel<TItemModel>
		where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>
	{
		try
		{
			var logger = _loggerFactory.CreateLogger<UmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository>>();
			var dialogUtility = _serviceProvider.GetRequiredService<IUmbrellaDialogService>();
			var repository = _serviceProvider.GetRequiredService<TRepository>();

			var service = new UmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository>(logger, dialogUtility, repository)
			{
				AutoRenderOnPageLoad = autoRenderOnPageLoad,
				CallGridStateHasChangedOnRefresh = callGridStateHasChangedOnRefresh,
				InitialSortDirection = initialSortDirection,
				InitialSortPropertyName = initialSortPropertyName,
				StateHasChangedDelegate = stateHasChangedDelegate
			};

			if (initialSortExpressions is not null)
				service.InitialSortExpressions = initialSortExpressions;

			if (loadPaginatedResultModelDelegate is not null)
				service.LoadPaginatedResultModelDelegate = loadPaginatedResultModelDelegate;

			return service;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { initialSortPropertyName, autoRenderOnPageLoad, callGridStateHasChangedOnRefresh, initialSortDirection }))
		{
			throw new UmbrellaBlazorException("There has been a problem creating the service.", exc);
		}
	}
}