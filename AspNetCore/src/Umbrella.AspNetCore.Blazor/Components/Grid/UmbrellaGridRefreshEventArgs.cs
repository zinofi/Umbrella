using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// The event arguments for refresh events raised by the <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
public readonly struct UmbrellaGridRefreshEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaGridRefreshEventArgs"/> struct.
	/// </summary>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	public UmbrellaGridRefreshEventArgs(int pageNumber, int pageSize, IReadOnlyCollection<SortExpressionDescriptor> sorters, IReadOnlyCollection<FilterExpressionDescriptor> filters)
	{
		PageNumber = pageNumber;
		PageSize = pageSize;
		Sorters = sorters;
		Filters = filters;
	}

	/// <summary>
	/// Gets the page number.
	/// </summary>
	public int PageNumber { get; }

	/// <summary>
	/// Gets the size of the page.
	/// </summary>
	public int PageSize { get; }

	/// <summary>
	/// Gets the sorters.
	/// </summary>
	public IReadOnlyCollection<SortExpressionDescriptor> Sorters { get; }

	/// <summary>
	/// Gets the filters.
	/// </summary>
	public IReadOnlyCollection<FilterExpressionDescriptor> Filters { get; }
}