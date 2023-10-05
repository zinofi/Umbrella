namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required to be a non-empty collection contingent on whether another property
/// on the same object as the property this attribute is being used on is <see langword="false"/>.
/// </summary>
/// <seealso cref="RequiredNonEmptyCollectionIfAttribute" />
public class RequiredNonEmptyCollectionIfFalseAttribute : RequiredNonEmptyCollectionIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfFalseAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public RequiredNonEmptyCollectionIfFalseAttribute(string dependentProperty)
		: base(dependentProperty, EqualityOperator.EqualTo, false)
	{
	}
}