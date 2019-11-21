using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for types that implement <see cref="IEnumerable{T}"/>.
	/// </summary>
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
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentInRange(itemsPerGroup, nameof(itemsPerGroup), 1);

			return source
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / itemsPerGroup)
				.Select(x => x.Select(v => v.Value));
		}

		/// <summary>
		/// Converts the source <see cref="IEnumerable{T}"/> to a <see cref="HashSet{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of the items in the collections.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="comparer">The comparer used by the hashset to determine item uniqueness.</param>
		/// <returns>The new <see cref="HashSet{T}"/>.</returns>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
		{
			Guard.ArgumentNotNull(source, nameof(source));

			return new HashSet<T>(source, comparer);
		}

		public static IEnumerable<int> CumulativeSum(this IEnumerable<int> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<int?> CumulativeSum(this IEnumerable<int?> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<float> CumulativeSum(this IEnumerable<float> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<float?> CumulativeSum(this IEnumerable<float?> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<double> CumulativeSum(this IEnumerable<double> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<double?> CumulativeSum(this IEnumerable<double?> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<decimal> CumulativeSum(this IEnumerable<decimal> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<decimal?> CumulativeSum(this IEnumerable<decimal?> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<long> CumulativeSum(this IEnumerable<long> source)
			=> CumulativeSum(source, (x, y) => x + y);

		public static IEnumerable<long?> CumulativeSum(this IEnumerable<long?> source)
			=> CumulativeSum(source, (x, y) => x + y);

		private static IEnumerable<T> CumulativeSum<T>(this IEnumerable<T> source, Func<T, T, T> addSelector)
		{
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNull(addSelector, nameof(addSelector));

			T sum = source.FirstOrDefault();

			yield return sum;

			foreach (T item in source.Skip(1))
			{
				sum = addSelector(sum, item);
				yield return sum;
			}
		}
	}
}