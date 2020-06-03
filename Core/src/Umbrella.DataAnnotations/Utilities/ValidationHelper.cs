using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations.Utilities
{
	internal static class ValidationHelper
	{
		private static readonly MinLengthAttribute _minLengthAttribute = new MinLengthAttribute(1);

		public static bool IsNonEmptyCollection(object value) => !(value is null) && _minLengthAttribute.IsValid(value);
	}
}