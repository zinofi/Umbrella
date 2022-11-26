namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should be less than or equal to the value of another named property on the same type.
/// </summary>
/// <seealso cref="IsAttribute" />
public class LessThanOrEqualToAttribute : IsAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LessThanOrEqualToAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public LessThanOrEqualToAttribute(string dependentProperty)
		: base(Operator.LessThanOrEqualTo, dependentProperty)
	{
	}
}