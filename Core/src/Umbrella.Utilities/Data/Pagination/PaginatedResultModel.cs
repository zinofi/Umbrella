using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Data.Pagination;

// TODO: Could this be a readonly struct?? Or even a record (not yet though!).
// NO. Can't be either yet because we need to be able to deserialize JSON to it.
// When we can full utilize .NET 5 we can do the above.

// TODO: Create a PaginatedResultModel<T>.Empty inside Umbrella to avoid allocations. Can only do this
// when it becomes readonly. Use C# 9 init too.

/// <summary>
/// Represents the result of a paginated query.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public class PaginatedResultModel<TItem>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PaginatedResultModel{TItem}"/> class.
	/// </summary>
	public PaginatedResultModel()
	{
		Items = Array.Empty<TItem>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PaginatedResultModel{TItem}"/> class.
	/// </summary>
	/// <param name="items">The items.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="totalCount">The total count.</param>
	public PaginatedResultModel(IReadOnlyCollection<TItem> items, int pageNumber, int pageSize, int totalCount)
	{
		Items = items;
		PageNumber = pageNumber;
		PageSize = pageSize;
		TotalCount = totalCount;
		MoreItems = PageNumber * PageSize < TotalCount;
	}

	/// <summary>
	/// Gets the items.
	/// </summary>
	public IReadOnlyCollection<TItem> Items { get; set; }

	/// <summary>
	/// Gets the page number.
	/// </summary>
	public int PageNumber { get; set; }

	/// <summary>
	/// Gets the size of the page.
	/// </summary>
	public int PageSize { get; set; }

	/// <summary>
	/// Gets the total count.
	/// </summary>
	public int TotalCount { get; set; }

	/// <summary>
	/// Gets a value indicating whether there are more items that can be retrieved on subsequent pages.
	/// </summary>
	public bool MoreItems { get; set; }
}