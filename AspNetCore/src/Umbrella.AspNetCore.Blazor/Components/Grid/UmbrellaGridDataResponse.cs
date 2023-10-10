namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// Represents a data response provided to the component as the result of a data request.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public readonly record struct UmbrellaGridDataResponse<TItem>(
	IReadOnlyCollection<TItem> Items,
	int? TotalCount = null,
	int? PageNumber = null,
	int? PageSize = null,
	bool CallStateHasChanged = true);