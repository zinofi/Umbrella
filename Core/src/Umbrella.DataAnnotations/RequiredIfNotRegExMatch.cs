namespace Umbrella.DataAnnotations;

/// Specifies that a data field is required contingent on whether another property
/// on the same object as the property this attribute is being used on does not match the specified regular expression.
public class RequiredIfNotRegExMatchAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfNotRegExMatchAttribute"/> class.
	/// </summary>
	/// <param name="dependentValue">The dependent value.</param>
	/// <param name="pattern">The pattern.</param>
	public RequiredIfNotRegExMatchAttribute(string dependentValue, string pattern)
		: base(dependentValue, EqualityOperator.NotRegExMatch, pattern)
	{
	}
}