namespace Umbrella.DataAnnotations;

public class RequiredIfRegExMatchAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfRegExMatchAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="pattern">The pattern.</param>
	public RequiredIfRegExMatchAttribute(string dependentProperty, string pattern)
		: base(dependentProperty, Operator.RegExMatch, pattern)
	{
	}
}