using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required contingent on whether another string property on the same
/// object is null, an empty string or only whitespace.
/// </summary>
/// <seealso cref="ContingentValidationAttribute" />
public sealed class RequiredIfEmptyAttribute : ContingentValidationAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfEmptyAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public RequiredIfEmptyAttribute(string dependentProperty)
		: base(dependentProperty) { }

	/// <inheritdoc />
	public override bool IsValid(object value, object? actualDependentPropertyValue, object model)
		=> !string.IsNullOrWhiteSpace((actualDependentPropertyValue ?? string.Empty).ToString()) || value is not null && !string.IsNullOrWhiteSpace(value.ToString());

	/// <inheritdoc />
	public override string DefaultErrorMessageFormat => "{0} is required due to {1} being empty.";
}