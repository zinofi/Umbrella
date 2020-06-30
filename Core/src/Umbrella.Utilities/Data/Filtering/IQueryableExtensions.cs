using System;
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
					UmbrellaDynamicCompare? dynamicCompare = filterExpression.Type switch
					{
						FilterType.Equal => UmbrellaDynamicCompare.Equal,
						FilterType.GreaterThan => UmbrellaDynamicCompare.GreaterThan,
						FilterType.GreaterThanOrEqual => UmbrellaDynamicCompare.GreaterThanOrEqual,
						FilterType.LessThan => UmbrellaDynamicCompare.LessThan,
						FilterType.LessThanOrEqual => UmbrellaDynamicCompare.LessThanOrEqual,
						FilterType.NotEqual => UmbrellaDynamicCompare.NotEqual,
						_ => null
					};

					Expression<Func<TItem, bool>> predicate = dynamicCompare != null
						? UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, dynamicCompare.Value, filterExpression.Value.ToString() ?? "")
						: UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, filterExpression.Type.ToString(), filterExpression.Value.ToString() ?? "");

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