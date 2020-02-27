using System;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Extends the <see cref="RangeAttribute" /> using either <see cref="double.MaxValue" /> or <see cref="int.MaxValue" /> as the default for convenience.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MinRangeAttribute : RangeAttribute
	{
		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="minimum">The minimum value.</param>
		public MinRangeAttribute(double minimum)
			: base(minimum, double.MaxValue)
		{
		}

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="minimum">The minimum value.</param>
		public MinRangeAttribute(int minimum)
			: base(minimum, int.MaxValue)
		{
		}
	}
}