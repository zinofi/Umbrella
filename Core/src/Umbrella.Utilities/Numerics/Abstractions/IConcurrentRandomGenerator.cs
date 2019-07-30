using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Numerics.Abstractions
{
	/// <summary>
	/// A thread-safe wrapper around the <see cref="Random"/> type.
	/// </summary>
	public interface IConcurrentRandomGenerator
	{
		/// <summary>
		/// Generates a unique collection of integers based on the specified <paramref name="min"/> and <paramref name="max"/> values with a size specified by the value of <paramref name="count"/>.
		/// </summary>
		/// <param name="min">The inclusive lower bound of the the random numbers returned.</param>
		/// <param name="max">The exclusive upper bound of the random numbers returned. <paramref name="max"/> must be greater than or equal to <paramref name="min"/>.</param>
		/// <param name="count">The size of the generated collection.</param>
		/// <param name="shuffle">Determines whether or not the returned collection will be randomly shuffled.</param>
		/// <returns>A collection containing the random numbers.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if either parameter value is less than zero, or if <paramref name="max"/> is less than <paramref name="min"/>.
		/// Also conditionally thrown, when <paramref name="max"/> is greater than <paramref name="min"/>, if the <paramref name="count"/>
		/// is less than one or is greater than the difference between <paramref name="min"/> and <paramref name="max"/> such that it will be
		/// impossible to generate a collection of the size of the specified <paramref name="count"/> value.
		/// </exception>
		IReadOnlyCollection<int> GenerateDistinctList(int min, int max, int count, bool shuffle);

		/// <summary>
		/// Gets the next random number based on the specified <paramref name="min"/> and <paramref name="max"/> values.
		/// </summary>
		/// <param name="min">The inclusive lower bound of the random number returned.</param>
		/// <param name="max">The exclusive upper bound of the random number returned. <paramref name="max"/> must be greater than or equal to <paramref name="min"/>.</param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to <paramref name="min"/> and less than <paramref name="max"/>;
		/// that is, the range of return values includes <paramref name="min"/> but not <paramref name="max"/>. If <paramref name="min"/>
		/// equals <paramref name="max"/>, <paramref name="min"/> is returned.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if either parameter value is less than zero, or if <paramref name="max"/> is less than <paramref name="min"/>.</exception>
		int Next(int min = 0, int max = 0);

		/// <summary>
		/// Returns a specified number of random elements from a sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The sequence to return elements from.</param>
		/// <param name="count">The number of elements to return.</param>
		/// <param name="shuffle">Determines whether or not the returned sequence will be randomly shuffled.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains the specified number of elements randomly selected from the sequence.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than one.</exception>
		IEnumerable<T> TakeRandom<T>(IEnumerable<T> source, int count, bool shuffle = false);
	}
}