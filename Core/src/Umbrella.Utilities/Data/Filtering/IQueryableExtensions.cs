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
			// TODO: Replicate this approach for the SortExpressions.
			// Might as well extend the IEnumerable versions too.
			IQueryable<TItem>? filteredItems = null;

			int filterExpressionsCount = filterExpressions?.Count() ?? 0;
			int additionalFiltersCount = additionalFilterExpressions?.Count() ?? 0;

			if (filterExpressionsCount > 0 || additionalFiltersCount > 0)
			{
				Expression<Func<TItem, bool>>? filterPredicate = null;
				Expression<Func<TItem, bool>>? primaryFilterPredicate = null;

				if (filterExpressionsCount > 0)
				{
					foreach (FilterExpression<TItem> filterExpression in filterExpressions!)
					{
						if (filterExpression == default || filterExpression.MemberPath is null)
							continue;

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

						Expression<Func<TItem, bool>> predicate = dynamicCompare is not null
							? UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, dynamicCompare.Value, filterExpression.Value?.ToString() ?? "")
							: UmbrellaDynamicQuery.CreatePredicate<TItem>(filterExpression.MemberPath, filterExpression.Type.ToString(), filterExpression.Value?.ToString() ?? "");

						if (filterExpression.IsPrimary)
						{
							primaryFilterPredicate = primaryFilterPredicate is null
								? predicate
								: primaryFilterPredicate.CombineAnd(predicate);
						}
						else
						{
							if (filterPredicate is null)
							{
								filterPredicate = predicate;
							}
							else if (predicate is not null)
							{
								filterPredicate = combinator switch
								{
									FilterExpressionCombinator.And => filterPredicate.CombineAnd(predicate),
									FilterExpressionCombinator.Or => filterPredicate.CombineOr(predicate),
									_ => throw new NotSupportedException($"The specified {nameof(FilterExpressionCombinator)} is not supported.")
								};
							}
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
						else if (filter is not null)
						{
							filterPredicate = combinator switch
							{
								FilterExpressionCombinator.And => filterPredicate.CombineAnd(filter),
								FilterExpressionCombinator.Or => filterPredicate.CombineOr(filter),
								_ => throw new NotSupportedException($"The specified {nameof(FilterExpressionCombinator)} is not supported.")
							};
						}
					}
				}

				filteredItems = items;

				if (primaryFilterPredicate is not null)
					filteredItems = filteredItems.Where(primaryFilterPredicate);

				if (filterPredicate is not null)
					filteredItems = filteredItems.Where(filterPredicate);
			}

			return filteredItems ?? items;
		}
	}
}