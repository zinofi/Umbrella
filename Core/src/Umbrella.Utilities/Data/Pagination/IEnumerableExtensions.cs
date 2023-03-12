namespace Umbrella.Utilities.Data.Pagination;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/> collections for pagination.
/// </summary>
public static class IEnumerableExtensions
{
	/// <summary>
	/// Applies pagination to the specified collection.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <returns>The collection with the pagination parameters applied.</returns>
	public static IEnumerable<TItem> ApplyPagination<TItem>(this IEnumerable<TItem> query, int pageNumber, int pageSize)
	{
		if (pageNumber > 0 && pageSize > 0)
		{
			int itemsToSkip = (pageNumber - 1) * pageSize;
			query = query.Skip(itemsToSkip).Take(pageSize);
		}

		return query;
	}
}