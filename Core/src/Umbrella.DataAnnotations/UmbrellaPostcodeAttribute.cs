using Umbrella.DataAnnotations.RegularExpressions;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field must match the regular expression for a UK Postcode.
/// </summary>
/// <seealso cref="RegularExpressionAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaPostcodeAttribute : RegularExpressionAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaPostcodeAttribute"/> class.
	/// </summary>
	public UmbrellaPostcodeAttribute()
		: base(PostcodeRegularExpressions.UKPostcodeRegexString)
	{
	}
}