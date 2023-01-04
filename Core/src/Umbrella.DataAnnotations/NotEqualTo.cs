namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should not be equal to the value of another named property on the same type.
/// </summary>
/// <seealso cref="IsAttribute" />
public class NotEqualToAttribute : IsAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="NotEqualToAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public NotEqualToAttribute(string dependentProperty)
		: base(EqualityOperator.NotEqualTo, dependentProperty)
	{
	}
}