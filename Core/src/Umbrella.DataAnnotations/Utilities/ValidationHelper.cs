using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations.Utilities
{
	internal static class ValidationHelper
	{
		private static readonly MinLengthAttribute _minLengthAttribute = new MinLengthAttribute(1);

		public static bool IsNonEmptyCollection(object value) => value switch
		{
			null => false,
			Array _ => _minLengthAttribute.IsValid(value),
			ICollection collection => collection.Count > 0,
			_ => throw new NotImplementedException("The value being validated must be of type Array or implement ICollection.")
		};
	}
}