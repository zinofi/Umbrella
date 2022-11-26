namespace Umbrella.DataAnnotations;

public class RequiredNonEmptyCollectionIfTrueAttribute : RequiredNonEmptyCollectionIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfTrueAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public RequiredNonEmptyCollectionIfTrueAttribute(string dependentProperty)
		: base(dependentProperty, Operator.EqualTo, true)
	{
	}
}