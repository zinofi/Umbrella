using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

		/// <summary>
		/// Performs the specified <paramref name="action"/> on each element of <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">A sequence of values to invoke the <paramref name="source"/> function on.</param>
		/// <param name="action">The action to perform on each element of <paramref name="source"/>.</param>
		/// <returns>An <see cref="IEnumerable{T}" /> whose elements are the result of invoking the transform function on each element of source.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNull(action, nameof(action));
			
			foreach (var item in source)
			{
				action(item);
			}

			return source;
		}

		/// <summary>
		/// Projects each element of a sequence into a new form asynchronously.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
		/// <param name="source">A sequence of values to invoke a transform function on.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
		/// <returns>
		/// An awaitable Task whose result contains an <see cref="IEnumerable{TResult}"/> whose elements are the result of 
		/// invoking the transform function on each element of <paramref name="source"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="selector"/> is null.</exception>
		public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNull(selector, nameof(selector));

			return await Task.WhenAll(source.Select(x => selector(x, cancellationToken)));
		}

		/// <summary>
		/// Parses the string in the <paramref name="items"/> to integers. Any items that cannot be parsed are discarded.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <returns>The parsed integers.</returns>
		public static IEnumerable<int> ParseIntegers(this IEnumerable<string> items)
		{
			foreach (string item in items)
			{
				if (int.TryParse(item, out int value))
					yield return value;
			}
		}

		// TODO: Add documentation and unit tests.
		// These methods return collections of the same length with each item set to the sum of the current item, plus the previous item,
		// e.g. 1, 2, 3,  4,  5,  6,  7,  8,  9, 10
		// ret. 1, 3, 6, 10, 15, 21, 28, 36, 45, 55
		// Same logic as used to generate Fibonacci sequence.
		// No idea why these exist though. CostsBudgIT maybe??? Check.
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