namespace Umbrella.DataAnnotations;

public class RequiredIfNotRegExMatchAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfNotRegExMatchAttribute"/> class.
	/// </summary>
	/// <param name="dependentValue">The dependent value.</param>
	/// <param name="pattern">The pattern.</param>
	public RequiredIfNotRegExMatchAttribute(string dependentValue, string pattern)
		: base(dependentValue, Operator.NotRegExMatch, pattern)
	{
	}
}