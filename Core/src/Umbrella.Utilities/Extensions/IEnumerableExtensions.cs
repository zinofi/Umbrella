using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Enumerations;

namespace Umbrella.Utilities.Extensions
{
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Splits a collection into groups
		/// </summary>
		/// <typeparam name="T">The type of the source collection</typeparam>
		/// <param name="source">The collection to be split into groups</param>
		/// <param name="itemsPerGroup">The number of items per group</param>
		/// <returns>A nested collection split into groups</returns>
		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int itemsPerGroup)
		{
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / itemsPerGroup)
                .Select(x => x.Select(v => v.Value));
		}

        public static IOrderedEnumerable<TSource> OrderBySortDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction, IComparer<TKey> comparer = null)
        {
            

            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return comparer == null ? source.OrderBy(keySelector) : source.OrderBy(keySelector, comparer);
                case SortDirection.Descending:
                    return comparer == null ? source.OrderByDescending(keySelector) : source.OrderByDescending(keySelector, comparer);
            }
        }
    }
}
