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
    /// <param name="maxLength">The maximum length of the genrated URL segment.</param>
    /// <returns>
    /// The URL segment.
    /// </returns>
    /// <exception cref="UmbrellaException">There was a problem generating the URL using the specified parameters.</exception>
    public string GenerateUrl(string text, int maxLength = 0)
    {
        try
        {
            if (text is null)
                return "";

            string normalizedString = text
                // Make lowercase
                .ToLowerInvariant()
                // Normalize the text
                .Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();
            int stringLength = normalizedString.Length;
            bool prevdash = false;
            int trueLength = 0;
            char c;

            for (int i = 0; i < stringLength; i++)
            {
                c = normalizedString[i];

                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    // Check if the character is a letter or a digit if the character is a
                    // international character remap it to an ascii valid character
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
                    // Check if the character is to be replaced by a hyphen but only if the last character wasn't
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

                // If we are at max length, stop parsing
                if (maxLength > 0 && trueLength >= maxLength)
                    break;
            }

            // Trim excess hyphens
            string result = stringBuilder.ToString().Trim('-');

            // Remove any excess character to meet maxlength criteria
            return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
        }
        catch (Exception exc) when (_log.WriteError(exc, new { text, maxLength }))
        {
            throw new UmbrellaException("There was a problem generating the URL using the specified parameters.", exc);
        }
    }
}