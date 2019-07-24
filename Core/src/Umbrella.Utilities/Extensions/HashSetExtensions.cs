using System.Collections.Generic;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// A set of extension methods for the <see cref="HashSet{T}"/> type.
	/// </summary>
	public static class HashSetExtensions
	{
		/// <summary>
		/// Adds a value to the specified <see cref="HashSet{T}"/> only if it is not null.
		/// </summary>
		/// <typeparam name="T">The type of elements in the <see cref="HashSet{T}"/>.</typeparam>
		/// <param name="hashSet">The <see cref="HashSet{T}"/> to which the <paramref name="value"/> will be added.</param>
		/// <param name="value">The value to be added to the <see cref="HashSet{T}"/>.</param>
		/// <returns>A <see langword="bool"/> indicating whether or not the value had been added to the <paramref name="hashSet"/>.</returns>
		public static bool AddNotNull<T>(this HashSet<T> hashSet, T value)
		{
			if (value != null)
				return hashSet.Add(value);

			return false;
		}

		/// <summary>
		/// Adds a string value to the specified <see cref="HashSet{T}"/> only if it is not null.
		/// In doing so, the string is also trimmed.
		/// </summary>
		/// <typeparam name="T">The type of elements in the <see cref="HashSet{T}"/>.</typeparam>
		/// <param name="hashSet">The <see cref="HashSet{T}"/> to which the <paramref name="value"/> will be added.</param>
		/// <param name="value">The value to be added to the <see cref="HashSet{T}"/>.</param>
		/// <returns>A <see langword="bool"/> indicating whether or not the value had been added to the <paramref name="hashSet"/>.</returns>
		public static bool AddNotNullTrim(this HashSet<string> hashSet, string value)
			=> AddNotNull(hashSet, value?.Trim());

		/// <summary>
		/// Adds a string value to the specified <see cref="HashSet{T}"/> only if it is not null.
		/// In doing do, the string value is trimmed and converted to lowercase using the rules of the invariant culture.
		/// </summary>
		/// <typeparam name="T">The type of elements in the <see cref="HashSet{T}"/>.</typeparam>
		/// <param name="hashSet">The <see cref="HashSet{T}"/> to which the <paramref name="value"/> will be added.</param>
		/// <param name="value">The value to be added to the <see cref="HashSet{T}"/>.</param>
		/// <returns>A <see langword="bool"/> indicating whether or not the value had been added to the <paramref name="hashSet"/>.</returns>
		public static bool AddNotNullTrimToLowerInvariant(this HashSet<string> hashSet, string value)
			=> AddNotNull(hashSet, value?.TrimToLowerInvariant());
	}
}