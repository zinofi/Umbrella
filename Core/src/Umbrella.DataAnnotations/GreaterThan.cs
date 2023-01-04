namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should be greater than the value of another named property on the same type.
/// </summary>
/// <seealso cref="IsAttribute" />
public class GreaterThanAttribute : IsAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public GreaterThanAttribute(string dependentProperty)
		: base(EqualityOperator.GreaterThan, dependentProperty)
	{
	}
}