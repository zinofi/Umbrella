using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAnnotations.Utilities
{
    internal static class ValidationHelper
    {
		private static readonly MinLengthAttribute _minLengthAttribute = new MinLengthAttribute(1);

		public static bool IsNonEmptyCollection(object value, ValidationContext validationContext)
		{
			return _minLengthAttribute.GetValidationResult(value, validationContext) == ValidationResult.Success;
		}
    }
}