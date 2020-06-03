using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	public class RequiredTrueAttribute : RequiredAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			=> value is true ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
	}
}