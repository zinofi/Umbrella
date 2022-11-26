namespace Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers.Navigation;

/// <summary>
/// The match type used to determine if a tag is active.
/// </summary>
public enum ActiveTagMatchType
{
	/// <summary>
	/// This specifies that the current URL will be matched if the tag's href attribute starts with the current URL.
	/// </summary>
	Hierarchical,

	/// <summary>
	/// This specifies that the current URL will be matched if the tag's href attribute exactly matches the current URL.
	/// </summary>
	Exact
}