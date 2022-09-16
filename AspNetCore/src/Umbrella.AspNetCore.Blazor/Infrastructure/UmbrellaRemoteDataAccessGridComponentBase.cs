// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with Blazor components that contain a single <see cref="UmbrellaGrid{TItem}"/> component in conjunction
/// with the <see cref="DataAccess.Remote"/> infrastructure.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TRepository">The type of the repository.</typeparam>
/// <seealso cref="UmbrellaGridComponentBase{TItemModel, TPaginatedResultModel}" />
public abstract class UmbrellaRemoteDataAccessGridComponentBase<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> : UmbrellaGridComponentBase<TItemModel, TPaginatedResultModel>
	where TItemModel : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
	where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>
{
	/// <summary>
	/// Gets or sets the repository.
	/// </summary>
	[Inject]
	protected TRepository Repository { get; set; } = null!;

	/// <inheritdoc/>
	protected override Task<IHttpCallResult<TPaginatedResultModel?>> LoadPaginatedResultModelAsync(int pageNumber, int pageSize, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null) => Repository.FindAllSlimAsync(pageNumber, pageSize, sorters: sorters, filters: filters);

	/// <summary>
	/// The event handler invoked when an item in the grid is to be deleted.
	/// </summary>
	/// <param name="item">The item.</param>
	public virtual async Task DeleteItemClick(TItemModel item)
	{
		try
		{
			Type type = typeof(TItemModel);
			string humanizedTypeName = type.Name.Humanize(LetterCasing.Title);

			bool confirmed = await DialogUtility.ShowConfirmDangerMessageAsync($"Are you sure you want to delete this {humanizedTypeName}? ", $"Delete {humanizedTypeName}");

			if (confirmed)
			{
				var result = await Repository.DeleteAsync(item.Id);

				if (result.Success)
				{
					int pageNumber = CurrentRefreshOptions?.PageNumber ?? GridInstance.PageNumber;
					int pageSize = CurrentRefreshOptions?.PageSize ?? GridInstance.PageSize;

					await DialogUtility.ShowSuccessMessageAsync($"The {humanizedTypeName} has been successfully deleted.", $"{humanizedTypeName} Deleted");
					await RefreshGridAsync(pageNumber, pageSize, CurrentRefreshOptions?.Sorters, CurrentRefreshOptions?.Filters);
				}
				else
				{
					await ShowProblemDetailsErrorMessageAsync(result.ProblemDetails, $"Delete {humanizedTypeName}");
				}
			}
		}
		catch (UmbrellaConcurrencyException)
		{
			await DialogUtility.ShowConcurrencyDangerMessageAsync();
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { item.Id }))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}
}