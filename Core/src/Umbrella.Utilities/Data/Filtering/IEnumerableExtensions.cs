using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/> collections for filtering.
	/// </summary>
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Applies the filter expressions to the collection.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="combinator">The filter combinator.</param>
		/// <param name="useCurrentCulture">if set to <c>true</c>, uses the current culture rules for any culture based comparisons, e.g. strings.</param>
		/// <returns>The filtered collection.</returns>
		public static IEnumerable<TItem> ApplyFilterExpressions<TItem>(this IEnumerable<TItem> items, IEnumerable<FilterExpression<TItem>>? filterExpressions, FilterExpressionCombinator combinator, bool useCurrentCulture = true)
		{
			IEnumerable<TItem>? filteredItems = null;

			if (filterExpressions?.Count() > 0)
			{
				var lstFilterPredicate = new List<Func<TItem, bool>>();

				foreach (var filterExpression in filterExpressions)
				{
					bool predicate(TItem x)
					{
						object? filterValue = filterExpression.Value;
						object? propertyValue = filterExpression.GetDelegate()?.Invoke(x);

						if (propertyValue is string strPropertyValue && filterValue is string strFilterValue)
						{
							StringComparison comparison = useCurrentCulture ? StringComparison.CurrentCultureIgnoreCase : StringComparison.InvariantCultureIgnoreCase;

							return filterExpression.Type switch
							{
								FilterType.Equal => strPropertyValue.Equals(strFilterValue, comparison),
								FilterType.NotEqual => !strPropertyValue.Equals(strFilterValue, comparison),
								FilterType.StartsWith => strPropertyValue.StartsWith(strFilterValue, comparison),
								FilterType.EndsWith => strPropertyValue.EndsWith(strFilterValue, comparison),
								FilterType.Contains => strPropertyValue.IndexOf(strFilterValue, comparison) > -1,
								_ => false
							};
						}

						return propertyValue?.Equals(filterValue) ?? false;
					}

					lstFilterPredicate.Add(predicate);
				}

				filteredItems = items;

				if (combinator == FilterExpressionCombinator.And)
				{
					lstFilterPredicate.ForEach(x => filteredItems = filteredItems.Where(y => x(y)));
				}
				else if (combinator == FilterExpressionCombinator.Or)
				{
					var lstItem = new HashSet<TItem>();
					lstFilterPredicate.ForEach(x => lstItem.UnionWith(filteredItems.Where(y => x(y))));
					filteredItems = lstItem;
				}
			}

			return filteredItems ?? items;
		}

		/// <summary>
		/// Converts a collection of <see cref="FilterExpression{TItem}"/> to a collection of <see cref="FilterExpressionDescriptor"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <returns>A <see cref="IEnumerable{FilterExpressionDescriptor}"/> collection.</returns>
		public static IEnumerable<FilterExpressionDescriptor> ToFilterExpressionDescriptors<TItem>(this IEnumerable<FilterExpression<TItem>> filterExpressions)
			=> filterExpressions.Select(x => (FilterExpressionDescriptor?)x).OfType<FilterExpressionDescriptor>();

		/// <summary>
		/// Finds a filter with the specified <paramref name="memberPath"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="filters">The filters.</param>
		/// <param name="memberPath">The member path.</param>
		/// <returns>The filter, if it exists.</returns>
		public static FilterExpression<TItem>? FindByMemberPath<TItem>(this IEnumerable<FilterExpression<TItem>>? filters, string memberPath)
		{
			FilterExpression<TItem>? result = filters?.SingleOrDefault(x => x.MemberPath?.Equals(memberPath, StringComparison.OrdinalIgnoreCase) ?? false);

			return result != default ? result : null;
		}
	}
}