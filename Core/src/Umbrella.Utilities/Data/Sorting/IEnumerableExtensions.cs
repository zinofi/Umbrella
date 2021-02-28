using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/> collections for sorting.
	/// </summary>
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Applies the sort expressions to the specified collection.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <param name="sortExpressions">The sort expressions.</param>
		/// <param name="defaultSortOrderExpression">The default sort order expression when <paramref name="sortExpressions"/> is null or empty.</param>
		/// <returns>The collection with the sort expressions applied to it.</returns>
		public static IEnumerable<TItem> ApplySortExpressions<TItem>(this IEnumerable<TItem> items, IEnumerable<SortExpression<TItem>>? sortExpressions, in SortExpression<TItem> defaultSortOrderExpression = default)
		{
			IOrderedEnumerable<TItem>? orderedItems = null;

			if (sortExpressions?.Count() > 0)
			{
				int i = 0;

				foreach (SortExpression<TItem> sortExpression in sortExpressions)
				{
					if (i++ is 0)
					{
						orderedItems = sortExpression.Direction == SortDirection.Ascending
							? items.OrderBy(sortExpression.GetDelegate())
							: items.OrderByDescending(sortExpression.GetDelegate());
					}
					else
					{
						orderedItems = sortExpression.Direction == SortDirection.Ascending
							? orderedItems.ThenBy(sortExpression.GetDelegate())
							: orderedItems.ThenByDescending(sortExpression.GetDelegate());
					}
				}
			}
			else if (defaultSortOrderExpression == default)
			{
				orderedItems = defaultSortOrderExpression.Direction is SortDirection.Ascending
							? items.OrderBy(defaultSortOrderExpression.GetDelegate())
							: items.OrderByDescending(defaultSortOrderExpression.GetDelegate());
			}

			return orderedItems ?? items;
		}

		/// <summary>
		/// Orders the collection by sort direction.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="direction">The direction.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The collection query.</returns>
		public static IOrderedEnumerable<TSource> OrderBySortDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction, IComparer<TKey>? comparer = null)
		{
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNull(keySelector, nameof(keySelector));

			return direction switch
			{
				SortDirection.Descending => comparer is null ? source.OrderByDescending(keySelector) : source.OrderByDescending(keySelector, comparer),
				_ => comparer is null ? source.OrderBy(keySelector) : source.OrderBy(keySelector, comparer),
			};
		}

		/// <summary>
		/// Converts a collection of <see cref="SortExpression{TItem}"/> to a collection of <see cref="SortExpressionDescriptor"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="sortExpressions">The sort expressions.</param>
		/// <returns>A <see cref="IEnumerable{SortExpressionDescriptor}"/> collection.</returns>
		public static IEnumerable<SortExpressionDescriptor> ToSortExpressionDescriptors<TItem>(this IEnumerable<SortExpression<TItem>> sortExpressions)
			=> sortExpressions.Select(x => (SortExpressionDescriptor?)x).OfType<SortExpressionDescriptor>();

		/// <summary>
		/// Finds a sorter with the specified <paramref name="memberPath"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="sorters">The sorters.</param>
		/// <param name="memberPath">The member path.</param>
		/// <returns>The sorter, if it exists.</returns>
		public static SortExpression<TItem>? FindByMemberPath<TItem>(this IEnumerable<SortExpression<TItem>>? sorters, string memberPath)
		{
			SortExpression<TItem>? result = sorters?.SingleOrDefault(x => x.MemberPath?.Equals(memberPath, StringComparison.OrdinalIgnoreCase) ?? false);

			return result != default(SortExpression<TItem>) ? result : null;
		}
	}
}