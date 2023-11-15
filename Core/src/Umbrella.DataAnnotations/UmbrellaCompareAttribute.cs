namespace Umbrella.DataAnnotations;

/// <summary>
/// An extension of the <see cref="CompareAttribute"/> with a fix to ensure that the <see cref="ValidationResult"/> that is
/// returned on validation failure contains the member name that the attribute targets.
/// </summary>
/// <seealso cref="CompareAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaCompareAttribute : CompareAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaCompareAttribute"/> class.
	/// </summary>
	/// <param name="otherProperty">The property to compare with the current property.</param>
	public UmbrellaCompareAttribute(string otherProperty)
		: base(otherProperty)
	{
	}

	/// <inheritdoc />
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));

		ValidationResult? result = base.IsValid(value, validationContext);

		if (result is null)
			return result;

		if (result == ValidationResult.Success)
			return result;

		if (!result.MemberNames.Any() && !string.IsNullOrEmpty(validationContext.MemberName))
			result = new ValidationResult(result.ErrorMessage, new[] { validationContext.MemberName });

		return result;
	}
}