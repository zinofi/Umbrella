using System;
using System.Linq;
using System.Linq.Expressions;
using Umbrella.Utilities.Sorting;

namespace Umbrella.DataAccess.Abstractions
{
	// TODO: V3 - Move to Core Utilities package.
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<TSource> OrderBySortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
        {
            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return source.OrderBy(keySelector);
                case SortDirection.Descending:
                    return source.OrderByDescending(keySelector);
            }
        }
    }
}