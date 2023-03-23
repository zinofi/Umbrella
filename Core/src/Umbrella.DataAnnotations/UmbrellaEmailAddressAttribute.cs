namespace Umbrella.DataAnnotations;

/// <summary>
/// An email address validation attribute that wraps the built in <see cref="EmailAddressAttribute"/>
/// and adds custom behaviour to allow null and empty strings to pass validation. The current implementation
/// of the <see cref="EmailAddressAttribute"/> requires that a value always be provided. This attribute should be used
/// in conjunction with the <see cref="RequiredAttribute"/>, or similar, when a value must be provided.
/// </summary>
/// <seealso cref="ValidationAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public class UmbrellaEmailAddressAttribute : ValidationAttribute
{
	private const string DefaultErrorMessage = "{0} must be a valid email address";
	private readonly EmailAddressAttribute _validator = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaEmailAddressAttribute"/> class.
	/// </summary>
	public UmbrellaEmailAddressAttribute()
		: base(DefaultErrorMessage)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object? value)
	{
		string? valueAsString = value?.ToString();

		return string.IsNullOrEmpty(valueAsString) || _validator.IsValid(valueAsString);
	}
}