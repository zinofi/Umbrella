namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required contingent on whether another property
/// on the same object as the property this attribute is being used on matches the specified regular expression.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
public sealed class RequiredIfRegExMatchAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfRegExMatchAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="pattern">The pattern.</param>
	public RequiredIfRegExMatchAttribute(string dependentProperty, string pattern)
		: base(dependentProperty, EqualityOperator.RegExMatch, pattern)
	{
		Pattern = pattern;
	}

	/// <summary>
	/// The pattern.
	/// </summary>
	public string Pattern { get; }
}