using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Sorting
{
    public static class IEnumerableExtensions
    {
		public static IEnumerable<TItem> ApplySortExpressions<TItem>(this IEnumerable<TItem> items, IEnumerable<SortExpression<TItem>> sortExpressions, in SortExpression<TItem> defaultSortOrderExpression = default)
		{
			IOrderedEnumerable<TItem> orderedItems = null;

			if (sortExpressions.Count() > 0)
			{
				int i = 0;

				foreach (SortExpression<TItem> sortExpression in sortExpressions)
				{
					if (i++ == 0)
					{
						orderedItems = sortExpression.Direction == SortDirection.Ascending
							? items.OrderBy(sortExpression.Func)
							: items.OrderByDescending(sortExpression.Func);
					}
					else
					{
						orderedItems = sortExpression.Direction == SortDirection.Ascending
							? orderedItems.ThenBy(sortExpression.Func)
							: orderedItems.ThenByDescending(sortExpression.Func);
					}
				}
			}
			else if (!defaultSortOrderExpression.Equals(default(SortExpression<TItem>)))
			{
				orderedItems = defaultSortOrderExpression.Direction == SortDirection.Ascending
							? items.OrderBy(defaultSortOrderExpression.Func)
							: items.OrderByDescending(defaultSortOrderExpression.Func);
			}

			return orderedItems ?? items;
		}

		public static IEnumerable<TItem> ApplyPagination<TItem>(this IEnumerable<TItem> query, int pageNumber, int pageSize)
		{
			if (pageNumber > 0 && pageSize > 0)
			{
				int itemsToSkip = (pageNumber - 1) * pageSize;
				query = query.Skip(itemsToSkip).Take(pageSize);
			}

			return query;
		}

		public static IEnumerable<SortExpressionSerializable> ToSortExpressionSerializables<TItem>(this IEnumerable<SortExpression<TItem>> sortExpressions)
			=> sortExpressions.Cast<SortExpressionSerializable>();
	}
}