﻿using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Grid;
using Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Services.Grid;

/// <summary>
/// A service that can be used with Blazor components that contain a <see cref="UmbrellaGrid{TItem}"/> component in conjunction with the <see cref="DataAccess.Remote"/> infrastructure.
/// Multiple instances of this service can be used to power multiple grids contained within a single Blazor component.
/// </summary>
/// <typeparam name="TItemModel">The type of the item model.</typeparam>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
/// <typeparam name="TRepository">The type of the repository.</typeparam>
/// <seealso cref="UmbrellaGridComponentService{TItemModel, TPaginatedResultModel}" />
public class UmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> : UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>, IUmbrellaRemoteDataAccessGridComponentService<TItemModel, TIdentifier, TPaginatedResultModel, TRepository> where TItemModel : class, IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
	where TPaginatedResultModel : PaginatedResultModel<TItemModel>
	where TRepository : class, IReadOnlyPaginatedSlimItemGenericRemoteRepository<TItemModel, TIdentifier, TPaginatedResultModel>, IDeleteItemGenericRemoteRepository<TIdentifier>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessGridComponentService{TItemModel, TIdentifier, TPaginatedResultModel, TRepository}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dialogUtility">The dialog utility.</param>
	/// <param name="repository">The repository.</param>
	internal UmbrellaRemoteDataAccessGridComponentService(
		ILogger<UmbrellaGridComponentService<TItemModel, TPaginatedResultModel>> logger,
		IUmbrellaDialogUtility dialogUtility,
		TRepository repository)
		: base(logger, dialogUtility)
	{
		Repository = repository;
		LoadPaginatedResultModelDelegate = (pageNumber, pageSize, sorters, filters) => Repository.FindAllSlimAsync(pageNumber, pageSize, sorters: sorters, filters: filters);
	}

	/// <inheritdoc />
	public TRepository Repository { get; }

	/// <inheritdoc />
	public async Task DeleteItemClick(TItemModel item)
	{
		try
		{
			string typeDisplayName = typeof(TItemModel).GetDisplayText();

			bool confirmed = await DialogUtility.ShowConfirmDangerMessageAsync($"Are you sure you want to delete this {typeDisplayName}? ", $"Delete {typeDisplayName}");

			if (confirmed)
			{
				var result = await Repository.DeleteAsync(item.Id);

				if (result.Success)
				{
					await DialogUtility.ShowSuccessMessageAsync($"The {typeDisplayName} has been successfully deleted.", $"{typeDisplayName} Deleted");
					await RefreshGridAsyncUsingCurrentRefreshOptions();
				}
				else
				{
					await ShowProblemDetailsErrorMessageAsync(result.ProblemDetails, $"Delete {typeDisplayName}");
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