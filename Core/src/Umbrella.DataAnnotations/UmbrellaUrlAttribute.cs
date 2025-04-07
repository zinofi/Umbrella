using Umbrella.DataAnnotations.RegularExpressions;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field must be a valid URL.
/// </summary>
/// <seealso cref="RegularExpressionAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaUrlAttribute : RegularExpressionAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaUrlAttribute"/> class.
	/// </summary>
	/// <param name="schemeRequired">Specifies whether the URL must include a scheme (http or https).</param>
	public UmbrellaUrlAttribute(bool schemeRequired = false)
		: base(schemeRequired ? UrlRegularExpressions.UrlSchemeRequiredRegexString : UrlRegularExpressions.UrlRegexString)
	{
	}

	/// <summary>
	/// Gets a value indicating whether the URL must include a scheme (http or https).
	/// </summary>
	public bool SchemeRequired { get; }
}