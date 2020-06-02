using System;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// A stricter version of the <see cref="MinLengthAttribute" /> which passes validation
	/// when the collection is null. This effectively incorporates the behaviour of <see cref="RequiredAttribute"/>
	/// and <see cref="MinLengthAttribute"/> in a single attritbute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RequiredNonEmptyCollectionAttribute : RequiredAttribute
	{
		private static readonly MinLengthAttribute _minLengthAttribute = new MinLengthAttribute(1);

		/// <inheritdoc />
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is null)
				return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

			var result = _minLengthAttribute.GetValidationResult(value, validationContext);

			return result != ValidationResult.Success
				? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
				: result;
		}
	}
}