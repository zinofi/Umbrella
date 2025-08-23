namespace Umbrella.Utilities.FriendlyUrl.Abstractions;

/// <summary>
/// A utility used to generate friendly URLs from a specified input string.
/// </summary>
public interface IFriendlyUrlGenerator
{
	/// <summary>
	/// Generates a friendly URL segment.
	/// </summary>
	/// <param name="text">The text to use to generate the URL segment.</param>
	/// <param name="maxLength">The maximum length of the generated URL segment.</param>
	/// <param name="replaceAmpersandWithAnd">If true, replaces all '&' characters with the word 'and' before slug generation.</param>
	/// <returns>
	/// The URL segment.
	/// </returns>
	string GenerateUrl(string text, int maxLength = 0, bool replaceAmpersandWithAnd = false);
}