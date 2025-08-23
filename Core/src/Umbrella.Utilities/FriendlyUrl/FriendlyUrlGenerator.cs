using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.FriendlyUrl.Abstractions;

namespace Umbrella.Utilities.FriendlyUrl;

// TODO: There is a bug in this in that a filename with an extension has it's period converted to a hyphen. This should not happen.

/// <summary>
/// A utility used to generate friendly URLs from a specified input string.
/// </summary>
/// <seealso cref="IFriendlyUrlGenerator" />
/// <remarks>See: https://www.johanbostrom.se/blog/how-to-create-a-url-and-seo-friendly-string-in-csharp-text-to-slug-generator for more details.</remarks>
public class FriendlyUrlGenerator : IFriendlyUrlGenerator
{
	private readonly ILogger _log;

	/// <summary>
	/// Initializes a new instance of the <see cref="FriendlyUrlGenerator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public FriendlyUrlGenerator(ILogger<FriendlyUrlGenerator> logger)
	{
		_log = logger;
	}

	/// <summary>
	/// Generates a friendly URL segment.
	/// </summary>
	/// <param name="text">The text to use to generate the URL segment.</param>
	/// <param name="maxLength">The maximum length of the generated URL segment.</param>
	/// <param name="replaceAmpersandWithAnd">If true, replaces all '&' characters with the word 'and' before slug generation.</param>
	/// <returns>
	/// The URL segment.
	/// </returns>
	/// <exception cref="UmbrellaException">There was a problem generating the URL using the specified parameters.</exception>
	public string GenerateUrl(string text, int maxLength = 0, bool replaceAmpersandWithAnd = false)
	{
		try
		{
			if (text is null)
				return string.Empty;

#if NET6_0_OR_GREATER
			if (replaceAmpersandWithAnd && text.Contains('&', StringComparison.Ordinal))
#else
			if (replaceAmpersandWithAnd && text.IndexOf('&') >= 0)
#endif
			{
				// Replace any '&' with the token ' and '. We'll collapse whitespace in the normalization phase.
				var sb = new StringBuilder(text.Length + 8);

				foreach (char ch in text)
				{
					if (ch == '&')
					{
						_ = sb.Append(' ').Append("and").Append(' ');
					}
					else
					{
						_ = sb.Append(ch);
					}
				}

				text = sb.ToString();
			}

			string normalizedString = text
				.ToLowerInvariant()
				.Normalize(NormalizationForm.FormD);

			var stringBuilder = new StringBuilder();
			int stringLength = normalizedString.Length;
			bool prevdash = false;
			int trueLength = 0;

			for (int i = 0; i < stringLength; i++)
			{
				char c = normalizedString[i];

				switch (CharUnicodeInfo.GetUnicodeCategory(c))
				{
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.DecimalDigitNumber:
						if (c < 128)
							_ = stringBuilder.Append(c);
						else
							_ = stringBuilder.Append(c.RemapInternationalCharacterToAscii());
						prevdash = false;
						trueLength = stringBuilder.Length;
						break;
					case UnicodeCategory.SpaceSeparator:
					case UnicodeCategory.ConnectorPunctuation:
					case UnicodeCategory.DashPunctuation:
					case UnicodeCategory.OtherPunctuation:
					case UnicodeCategory.MathSymbol:
						if (!prevdash)
						{
							_ = stringBuilder.Append('-');
							prevdash = true;
							trueLength = stringBuilder.Length;
						}

						break;
				}

				if (maxLength > 0 && trueLength >= maxLength)
					break;
			}

			string result = stringBuilder.ToString().Trim('-');

			return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
		}
		catch (Exception exc) when (_log.WriteError(exc, new { text, maxLength, replaceAmpersandWithAnd }))
		{
			throw new UmbrellaException("There was a problem generating the URL using the specified parameters.", exc);
		}
	}
}