namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should be equal to the value of another named property on the same type.
/// </summary>
/// <seealso cref="IsAttribute" />
public sealed class EqualToAttribute : IsAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EqualToAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public EqualToAttribute(string dependentProperty)
		: base(EqualityOperator.EqualTo, dependentProperty)
	{
	}
}