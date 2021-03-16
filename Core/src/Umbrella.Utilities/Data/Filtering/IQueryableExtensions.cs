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
		/// <param name="additionalFilterExpressions">Optional additional filter expressions that are too complex to model using the <see cref="FilterExpression{TItem}"/> type.</param>
		/// <returns>The query.</returns>
		public static IQueryable<TItem> ApplyFilterExpressions<TItem>(this IQueryable<TItem> items, IEnumerable<FilterExpression<TItem>>? filterExpressions, FilterExpressionCombinator combinator, IEnumerable<Expression<Func<TItem, bool>>>? additionalFilterExpressions = null)
		{
			IQueryable<TItem>? filteredItems = null;

			int filterExpressionsCount = filterExpressions?.Count() ?? 0;
			int additionalFiltersCount = additionalFilterExpressions?.Count() ?? 0;

			if (filterExpressionsCount > 0 || additionalFiltersCount > 0)
			{
				Expression<Func<TItem, bool>>? filterPredicate = null;

				if (filterExpressionsCount > 0)
				{
					foreach (FilterExpression<TItem> filterExpression in filterExpressions!)
					{
						if (filterExpression == default || filterExpression.MemberPath is null || filterExpression.Value is null)
							continue;

						Expression<Func<TItem, bool>>? predicate = null;

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

						predicate = dynamicCompare != null
							? UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, dynamicCompare.Value, filterExpression.Value.ToString() ?? "")
							: UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, filterExpression.Type.ToString(), filterExpression.Value.ToString() ?? "");

						if (filterPredicate is null)
						{
							filterPredicate = predicate;
						}
						else if (predicate != null)
						{
							filterPredicate = combinator switch
							{
								FilterExpressionCombinator.And => filterPredicate.And(predicate),
								FilterExpressionCombinator.Or => filterPredicate.Or(predicate),
								_ => throw new NotSupportedException($"The specified {nameof(FilterExpressionCombinator)} is not supported.")
							};
						}
					}
				}

				if (additionalFiltersCount > 0)
				{
					foreach (var filter in additionalFilterExpressions!)
					{
						if (filterPredicate is null)
						{
							filterPredicate = filter;
						}
						else if (filter != null)
						{
							filterPredicate = combinator switch
							{
								FilterExpressionCombinator.And => filterPredicate.And(filter),
								FilterExpressionCombinator.Or => filterPredicate.Or(filter),
								_ => throw new NotSupportedException($"The specified {nameof(FilterExpressionCombinator)} is not supported.")
							};
						}
					}
				}

				filteredItems = items.Where(filterPredicate);
			}

			return filteredItems ?? items;
		}
	}
}