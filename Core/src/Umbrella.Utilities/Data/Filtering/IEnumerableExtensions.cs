﻿using System;
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
		/// <param name="useCurrentCulture">if set to <c>true</c>, uses the current culture rules for any culture based comparisons, e.g. strings.</param>
		/// <returns>The filtered collection.</returns>
		public static IEnumerable<TItem> ApplyFilterExpressions<TItem>(this IEnumerable<TItem> items, IEnumerable<FilterExpression<TItem>> filterExpressions, bool useCurrentCulture = true)
		{
			IEnumerable<TItem> filteredItems = null;

			if (filterExpressions?.Count() > 0)
			{
				foreach (var filterExpression in filterExpressions)
				{
					items = items.Where(x =>
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
					});
				}
			}

			return filteredItems ?? items;
		}
	}
}