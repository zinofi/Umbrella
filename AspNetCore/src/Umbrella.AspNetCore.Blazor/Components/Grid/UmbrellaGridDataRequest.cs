using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// Represents a data request raised by the <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
public readonly record struct UmbrellaGridDataRequest(
	int PageNumber,
	int PageSize,
	IReadOnlyCollection<SortExpressionDescriptor> Sorters,
	IReadOnlyCollection<FilterExpressionDescriptor> Filters);