using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	public static class IComparableExtensions
	{
		public static bool IsValidRange<T>(this T? value, T min, T max, bool allowNull = true) where T : struct, IComparable<T>
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