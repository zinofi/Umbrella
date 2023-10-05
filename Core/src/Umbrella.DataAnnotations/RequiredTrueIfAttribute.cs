namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field value is conditionally required and that if the condition is satisfied, it must have a value of <see langword="true" />.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
public class RequiredTrueIfAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredTrueIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="dependentValue"></param>
	public RequiredTrueIfAttribute(string dependentProperty, object dependentValue)
		: base(dependentProperty, EqualityOperator.EqualTo, dependentValue)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object value, object actualDependentPropertyValue, object model)
		=> !Metadata.IsValid(actualDependentPropertyValue, ComparisonValue, ReturnTrueOnEitherNull) || value is true;
}