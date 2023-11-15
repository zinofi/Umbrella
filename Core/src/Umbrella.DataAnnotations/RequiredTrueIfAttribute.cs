namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field value is conditionally required and that if the condition is satisfied, it must have a value of <see langword="true" />.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
public sealed class RequiredTrueIfAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredTrueIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="comparisonValue">The comparison value.</param>
	public RequiredTrueIfAttribute(string dependentProperty, object comparisonValue)
		: base(dependentProperty, EqualityOperator.EqualTo, comparisonValue)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object? value, object? actualDependentPropertyValue, object model)
		=> !Metadata.IsValid(actualDependentPropertyValue, ComparisonValue, ReturnTrueOnEitherNull) || value is true;
}