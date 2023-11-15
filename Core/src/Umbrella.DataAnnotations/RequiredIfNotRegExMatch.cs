namespace Umbrella.DataAnnotations;

/// Specifies that a data field is required contingent on whether another property
/// on the same object as the property this attribute is being used on does not match the specified regular expression.
public sealed class RequiredIfNotRegExMatchAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfNotRegExMatchAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="pattern">The pattern.</param>
	public RequiredIfNotRegExMatchAttribute(string dependentProperty, string pattern)
		: base(dependentProperty, EqualityOperator.NotRegExMatch, pattern)
	{
		Pattern = pattern;
	}

	/// <summary>
	/// The pattern.
	/// </summary>
	public string Pattern { get; }
}