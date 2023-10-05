// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.Data.Sorting;

/// <summary>
/// Extensions for <see cref="IQueryable{T}"/> collections for sorting.
/// </summary>
public static class IQueryableExtensions
{
	/// <summary>
	/// Applies the sort expressions to the specified query.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="items">The items.</param>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <param name="defaultSortOrderExpression">The default sort order expression when <paramref name="sortExpressions"/> is null or empty.</param>
	/// <returns>The query with the sort expressions applied to it.</returns>
	public static IQueryable<TItem> ApplySortExpressions<TItem>(this IQueryable<TItem> items, IEnumerable<SortExpression<TItem>>? sortExpressions, in SortExpression<TItem> defaultSortOrderExpression = default)
	{
		IOrderedQueryable<TItem>? orderedQuery = null;

		if (sortExpressions?.Count() > 0)
		{
			int i = 0;

			foreach (SortExpression<TItem> sortExpression in sortExpressions)
			{
				orderedQuery = i++ == 0
					? sortExpression.Direction == SortDirection.Ascending
						? items.OrderBy(sortExpression.Expression)
						: items.OrderByDescending(sortExpression.Expression)
					: sortExpression.Direction == SortDirection.Ascending
						? orderedQuery.ThenBy(sortExpression.Expression)
						: orderedQuery.ThenByDescending(sortExpression.Expression);
			}
		}
		else if (defaultSortOrderExpression == default)
		{
			orderedQuery = defaultSortOrderExpression.Direction == SortDirection.Ascending
						? items.OrderBy(defaultSortOrderExpression.Expression)
						: items.OrderByDescending(defaultSortOrderExpression.Expression);
		}

		return orderedQuery ?? items;
	}

	/// <summary>
	/// Orders the query by sort direction.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="source">The source.</param>
	/// <param name="keySelector">The key selector.</param>
	/// <param name="direction">The direction.</param>
	/// <param name="comparer">The comparer.</param>
	/// <returns>The ordered query.</returns>
	public static IOrderedQueryable<TSource> OrderBySortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection direction, IComparer<TKey>? comparer = null)
	{
		Guard.IsNotNull(source, nameof(source));
		Guard.IsNotNull(keySelector, nameof(keySelector));

		return direction switch
		{
			SortDirection.Descending => comparer is null ? source.OrderByDescending(keySelector) : source.OrderByDescending(keySelector, comparer),
			_ => comparer is null ? source.OrderBy(keySelector) : source.OrderBy(keySelector, comparer),
		};
	}
}