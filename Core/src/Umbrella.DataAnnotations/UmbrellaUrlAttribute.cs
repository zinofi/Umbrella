using Umbrella.DataAnnotations.RegularExpressions;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field must be a valid URL.
/// </summary>
/// <seealso cref="RegularExpressionAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public class UmbrellaUrlAttribute : RegularExpressionAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaUrlAttribute"/> class.
	/// </summary>
	public UmbrellaUrlAttribute()
		: base(UrlRegularExpressions.UrlRegexString)
	{
	}
}