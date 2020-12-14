using System;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for use with <see cref="IComparable{T}"/> instances.
	/// </summary>
	public static class IComparableExtensions
	{
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
}