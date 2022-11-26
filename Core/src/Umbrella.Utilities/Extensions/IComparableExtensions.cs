using System;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with <see cref="IComparable{T}"/> instances.
/// </summary>
public static class IComparableExtensions
{
	/// <summary>
	/// Determines whether the specified value is between the specified <paramref name="min"/> and <paramref name="max"/> values inclusive.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <param name="min">The minimum.</param>
	/// <param name="max">The maximum.</param>
	/// <param name="allowNull">if set to <see langword="true"/> allows the <paramref name="value"/> to be null and returns <see langword="true"/> if that is the case.</param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="value"/> is between the specified range; otherwise <see langword="false"/>.
	/// </returns>
	public static bool IsValidRange<T>(this T? value, T min, T max, bool allowNull = true)
		where T : struct, IComparable<T>
	{
		if (!value.HasValue)
			return allowNull;

		T unwrappedValue = value.Value;

		bool greaterThanOrEqualToMin = unwrappedValue.CompareTo(min) >= 0;
		bool lessThanOrEqualToMax = unwrappedValue.CompareTo(max) <= 0;

		return greaterThanOrEqualToMin && lessThanOrEqualToMax;
	}
}