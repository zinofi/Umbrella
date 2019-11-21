using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="items">The items.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="defaultFilterExpression">The default filter expression.</param>
		/// <param name="useCurrentCulture">if set to <c>true</c>, uses the current culture rules for any culture based comparisons, e.g. strings.</param>
		/// <returns>The filtered collection.</returns>
		public static IEnumerable<TItem> ApplyFilterExpressions<TItem, TProperty>(this IEnumerable<TItem> items, IEnumerable<FilterExpression<TItem, TProperty>> filterExpressions, FilterExpression<TItem, TProperty> defaultFilterExpression = default, bool useCurrentCulture = true)
		{
			IEnumerable<TItem> filteredItems = null;

			if (filterExpressions?.Count() > 0)
			{
				foreach (var filterExpression in filterExpressions)
				{
					items = items.Where(x => ApplySingleExpression(filterExpression, useCurrentCulture, x));
				}
			}
			else if (defaultFilterExpression == default)
			{
				items = items.Where(x => ApplySingleExpression(defaultFilterExpression, useCurrentCulture, x));
			}

			return filteredItems ?? items;
		}

		private static bool ApplySingleExpression<TItem, TProperty>(FilterExpression<TItem, TProperty> filterExpression, bool useCurrentCulture, TItem item)
		{
			TProperty filterValue = filterExpression.Value;
			TProperty propertyValue = filterExpression.Func(item);

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