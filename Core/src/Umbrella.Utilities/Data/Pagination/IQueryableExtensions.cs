using System.Linq;

namespace Umbrella.Utilities.Data.Pagination
{
	/// <summary>
	/// Extensions for <see cref="IQueryable{T}"/> collections for pagination.
	/// </summary>
	public static class IQueryableExtensions
	{
		/// <summary>
		/// Applies pagination to the specified query.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="query">The query.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns>The query with the pagination parameters applied.</returns>
		public static IQueryable<TItem> ApplyPagination<TItem>(this IQueryable<TItem> query, int pageNumber, int pageSize)
		{
			if (pageNumber > 0 && pageSize > 0)
			{
				int itemsToSkip = (pageNumber - 1) * pageSize;
				query = query.Skip(itemsToSkip).Take(pageSize);
			}

			return query;
		}
	}
}