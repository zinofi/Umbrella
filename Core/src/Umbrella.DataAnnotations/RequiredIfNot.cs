namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required contingent on whether another property
/// on the same object as the property this attribute is being used on does not match the specified dependent value.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
public class RequiredIfNotAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfNotAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="dependentValue">The dependent value.</param>
	public RequiredIfNotAttribute(string dependentProperty, object dependentValue)
		: base(dependentProperty, EqualityOperator.NotEqualTo, dependentValue)
	{
	}
}