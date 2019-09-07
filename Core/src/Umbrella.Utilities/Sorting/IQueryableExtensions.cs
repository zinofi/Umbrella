using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Sorting
{
    public static class IQueryableExtensions
    {
		public static IQueryable<TItem> ApplySortExpressions<TItem>(this IQueryable<TItem> items, IEnumerable<SortExpression<TItem>> sortExpressions, in SortExpression<TItem> defaultSortOrderExpression = default)
		{
			IOrderedQueryable<TItem> orderedQuery = null;

			if (sortExpressions.Count() > 0)
			{
				int i = 0;

				foreach (SortExpression<TItem> sortExpression in sortExpressions)
				{
					if (i++ == 0)
					{
						orderedQuery = sortExpression.Direction == SortDirection.Ascending
							? items.OrderBy(sortExpression.Expression)
							: items.OrderByDescending(sortExpression.Expression);
					}
					else
					{
						orderedQuery = sortExpression.Direction == SortDirection.Ascending
							? orderedQuery.ThenBy(sortExpression.Expression)
							: orderedQuery.ThenByDescending(sortExpression.Expression);
					}
				}
			}
			else if (!defaultSortOrderExpression.Equals(default(SortExpression<TItem>)))
			{
				orderedQuery = defaultSortOrderExpression.Direction == SortDirection.Ascending
							? items.OrderBy(defaultSortOrderExpression.Expression)
							: items.OrderByDescending(defaultSortOrderExpression.Expression);
			}

			return orderedQuery ?? items;
		}

		public static IQueryable<TItem> ApplyPagination<TItem>(this IQueryable<TItem> query, int pageNumber, int pageSize)
		{
			if (pageNumber > 0 && pageSize > 0)
			{
				int itemsToSkip = (pageNumber - 1) * pageSize;
				query = query.Skip(itemsToSkip).Take(pageSize);
			}

			return query;
		}

		public static IOrderedQueryable<TSource> OrderBySortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
		{
			switch (direction)
			{
				default:
				case SortDirection.Ascending:
					return source.OrderBy(keySelector);
				case SortDirection.Descending:
					return source.OrderByDescending(keySelector);
			}
		}
	}
}