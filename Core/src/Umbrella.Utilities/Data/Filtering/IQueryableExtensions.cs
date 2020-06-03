﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbrella.Utilities.Expressions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Extensions for <see cref="IQueryable{T}"/> collections for filtering.
	/// </summary>
	public static class IQueryableExtensions
	{
		/// <summary>
		/// Applies the filter expressions to the specified query.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="combinator">The combinator.</param>
		/// <returns>The query.</returns>
		public static IQueryable<TItem> ApplyFilterExpressions<TItem>(this IQueryable<TItem> items, IEnumerable<FilterExpression<TItem>> filterExpressions, FilterExpressionCombinator combinator)
		{
			IQueryable<TItem> filteredItems = null;

			if (filterExpressions?.Count() > 0)
			{
				int i = 0;
				Expression<Func<TItem, bool>> filterPredicate = null;

				foreach (FilterExpression<TItem> filterExpression in filterExpressions)
				{
					DynamicCompare? dynamicCompare = filterExpression.Type switch
					{
						FilterType.Equal => DynamicCompare.Equal,
						FilterType.GreaterThan => DynamicCompare.GreaterThan,
						FilterType.GreaterThanOrEqual => DynamicCompare.GreaterThanOrEqual,
						FilterType.LessThan => DynamicCompare.LessThan,
						FilterType.LessThanOrEqual => DynamicCompare.LessThanOrEqual,
						FilterType.NotEqual => DynamicCompare.NotEqual,
						_ => null
					};

					Expression<Func<TItem, bool>> predicate = dynamicCompare != null
						? DynamicQuery.CreatePredicate<TItem>(filterExpression.MemberName, dynamicCompare.Value, filterExpression.Value.ToString() ?? "")
						: DynamicQuery.CreatePredicate<TItem>(filterExpression.MemberName, filterExpression.Type.ToString(), filterExpression.Value.ToString() ?? "");

					if (i++ == 0)
					{
						filterPredicate = predicate;
					}
					else if (filterPredicate != null)
					{
						filterPredicate = combinator switch
						{
							FilterExpressionCombinator.And => filterPredicate.And(predicate),
							FilterExpressionCombinator.Or => filterPredicate.Or(predicate),
							_ => throw new NotSupportedException($"The specified {nameof(FilterExpressionCombinator)} is not supported.")
						};
					}
				}

				filteredItems = items.Where(filterPredicate);
			}

			return filteredItems ?? items;
		}
	}
}