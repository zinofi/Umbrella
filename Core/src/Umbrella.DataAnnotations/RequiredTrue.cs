namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field value is both required and that it must have a value of <see langword="true" />.
/// </summary>
public class RequiredTrueAttribute : ValidationAttribute
{
	/// <inheritdoc />
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		=> value is true ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), !string.IsNullOrWhiteSpace(validationContext.MemberName) ? new[] { validationContext!.MemberName } : Array.Empty<string>());
}