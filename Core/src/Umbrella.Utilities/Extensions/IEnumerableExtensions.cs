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

        /// <summary>
        /// This is a convenience method to allow ordering of collections to be expressed succinctly. However, this method internally
        /// compiles the supplied <paramref name="keySelector"/> expression to a delegate which is not cached so this method is slower
        /// than writing out a tertiary statement longhand to call either <see cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
        /// or <see cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
        /// based on the value of <paramref name="direction"/>.
        /// If performance is ultra critical in the place you want to call this method consider going with the manual longhand approach instead!
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderBySortDirection<TSource, TKey>(this IEnumerable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
        {
            var func = keySelector.Compile();

            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return source.OrderBy(func);
                case SortDirection.Descending:
                    return source.OrderByDescending(func);
            }
        }
    }
}
