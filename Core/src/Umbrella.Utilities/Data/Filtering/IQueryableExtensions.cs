using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Extensions for <see cref="IQueryable{T}"/> collections for filtering.
	/// </summary>
	public static class IQueryableExtensions
	{
		/// <summary>
		/// Applies the filter expressions to the query.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="useCurrentCulture">if set to <c>true</c>, uses the current culture rules for any culture based comparisons, e.g. strings.</param>
		/// <returns>The filtered query.</returns>
		public static IQueryable<TItem> ApplyFilterExpressions<TItem>(this IQueryable<TItem> items, IEnumerable<FilterExpression<TItem>> filterExpressions, bool useCurrentCulture = true)
		{
			IQueryable<TItem> filteredItems = null;

			if (filterExpressions?.Count() > 0)
			{
				foreach (var filterExpression in filterExpressions)
				{
					// TODO: Will need to translate this manually somehow!
					items = items.Where(x => ApplyFilters(useCurrentCulture, x, filterExpression));
				}
			}

			return filteredItems ?? items;
		}

		private static bool ApplyFilters<TItem>(bool useCurrentCulture, TItem x, FilterExpression<TItem> filterExpression)
		{
			object filterValue = filterExpression.Value;
			object propertyValue = filterExpression.Func(x);

			if (propertyValue is string strPropertyValue && filterValue is string strFilterValue)
			{
				StringComparison comparison = useCurrentCulture ? StringComparison.CurrentCultureIgnoreCase : StringComparison.InvariantCultureIgnoreCase;

				return filterExpression.Type switch
				{
					FilterType.Exact => strPropertyValue.Equals(strFilterValue, comparison),
					FilterType.StartsWith => strPropertyValue.StartsWith(strFilterValue, comparison),
					FilterType.EndsWith => strPropertyValue.EndsWith(strFilterValue, comparison),
					FilterType.Contains => strPropertyValue.IndexOf(strFilterValue, comparison) > -1,
					_ => false
				};
			}

			return propertyValue.Equals(filterValue);
		}
	}
}