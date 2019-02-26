using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Globalization;

namespace Umbrella.Utilities.Extensions
{
    // TODO: Rework some of this internally to take advantage of the new Span stuff.
    public static class StringExtensions
    {
        private static readonly Regex s_HtmlTagPatternRegex = new Regex(c_HtmlTagPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex s_EllipsisPatternRegex = new Regex(c_EllipsisPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const string c_HtmlTagPattern = @"<.*?>";
        private const string c_EllipsisPattern = @"[\.]+$";

        public static string Truncate(this string text, int maxLength)
        {
            //Ensure we strip out HTML tags
            if (!string.IsNullOrEmpty(text))
                text = text.StripHtml();

            if (text.Length < maxLength)
                return text;

            return AppendEllipsis(text.Substring(0, maxLength - 3));
        }

        public static string TruncateAtWord(this string value, int length)
        {
            //Ensure we strip out HTML tags
            if (!string.IsNullOrEmpty(value))
                value = value.StripHtml();

            if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
                return value;

            return AppendEllipsis(value.Substring(0, value.IndexOf(" ", length)));
        }

        public static string StripNbsp(this string value) => !string.IsNullOrEmpty(value) ? value.Replace("&nbsp;", "") : null;

        public static bool IsValidLength(this string value, int minLength, int maxLength, bool allowNull = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return allowNull;

            return value.Length >= minLength && value.Length <= maxLength;
        }

        public static string StripHtml(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return s_HtmlTagPatternRegex.Replace(value, string.Empty);
        }

        public static string ToCamelCase(this string value) => ToCamelCaseInternal(value, false);
        public static string ToCamelCaseInvariant(this string value) => ToCamelCaseInternal(value, true);

        public static string ToCamelCaseInternal(string value, bool useInvariantCulture)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            Func<string, string> stringLower = null;
            Func<char, char> charLower = null;

            if (useInvariantCulture)
            {
                stringLower = x => x.ToLowerInvariant();
                charLower = char.ToLowerInvariant;
            }
            else
            {
                stringLower = x => x.ToLower();
                charLower = char.ToLower;
            }

            if (value.Length == 1)
                return stringLower(value);

            //If 1st char is already in lowercase, return the value untouched
            if (char.IsLower(value[0]))
                return value;

            char[] buffer = new char[value.Length];

            bool stop = false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!stop)
                {
                    if (char.IsUpper(value[i]))
                    {
                        buffer[i] = charLower(value[i]);
                        continue;
                    }
                    else if (i > 1)
                    {
                        //Encountered first lowercase char
                        //Check previous char and see if that was uppercase before we made it lowercase
                        char previous = value[i - 1];
                        if (char.IsUpper(previous))
                        {
                            buffer[i - 1] = previous;
                            stop = true;
                        }
                    }
                }

                buffer[i] = value[i];
            }

            return new string(buffer);
        }

        private static string AppendEllipsis(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            value += "...";

            return s_EllipsisPatternRegex.Replace(value, "...");
        }

        public static string ConvertHtmlBrTagsToLineBreaks(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            StringBuilder sb = new StringBuilder(value);
            sb.ConvertHtmlBrTagsToReplacement("\n");

            return sb.ToString();
        }

        public static string Clean(this string value, bool convertBrTagsToNl = false, bool trim = true, bool trimNewLines = true, bool stripHtml = true, bool stripNbsp = true, bool decodeHtmlEncodedLineBreaks = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            StringBuilder sb = new StringBuilder(value);

            if (convertBrTagsToNl)
                sb.ConvertHtmlBrTagsToReplacement("\n");
            else
                sb.ConvertHtmlBrTagsToReplacement("");

            if (trim)
                sb.Trim();

            bool trimAgain = false;

            if (decodeHtmlEncodedLineBreaks)
            {
                sb.Replace("&#10;", "\n")
                    .Replace("&#13;", "\r");
            }

            //Replace the following in strings
            sb.Replace("&amp;", "&");
            sb.Replace("&#39;", "'");
            sb.Replace("&quot;", "\"");
            sb.Replace("&#8216;", "'"); //Left quote
            sb.Replace("&#8217;", "'"); //Right quote

            if (trimNewLines)
            {
                sb.Trim('\r').Trim('\n');
                trimAgain = true;
            }

            if (stripNbsp)
            {
                sb.Replace("&nbsp;", "");
                trimAgain = true;
            }

            string cleanedValue = sb.ToString();

            if (stripHtml)
                cleanedValue = cleanedValue.StripHtml();

            if (trim && trimAgain)
                cleanedValue = cleanedValue.Trim();

            return cleanedValue;
        }

        private static string ToSnakeCase(this string value, bool lowerCase = true, bool removeWhiteSpace = true)
            => ToSnakeCaseInternal(value, lowerCase, removeWhiteSpace, false);

        private static string ToSnakeCaseInvariant(this string value, bool lowerCase = true, bool removeWhiteSpace = true)
        => ToSnakeCaseInternal(value, lowerCase, removeWhiteSpace, true);

        private static string ToSnakeCaseInternal(string value, bool lowerCase, bool removeWhiteSpace, bool useInvariantCulture)
        {
            Func<string, string> stringLower = null;
            Func<char, char> charLower = null;

            if (useInvariantCulture)
            {
                stringLower = x => x.ToLowerInvariant();
                charLower = char.ToLowerInvariant;
            }
            else
            {
                stringLower = x => x.ToLower();
                charLower = char.ToLower;
            }

            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.Length == 1)
                return lowerCase ? stringLower(value) : value;

            List<char> buffer = new List<char>(value.Length)
            {
                lowerCase ? charLower(value[0]) : value[0]
            };

            for (int i = 1; i < value.Length; i++)
            {
                char current = value[i];

                if (removeWhiteSpace && char.IsWhiteSpace(current))
                    continue;

                if (char.IsUpper(value[i]))
                {
                    if (lowerCase)
                        current = charLower(current);

                    buffer.Add('_');
                }

                buffer.Add(current);
            }

            //TODO: Can this be made more efficient somehow by avoiding the intermediate array??
            return new string(buffer.ToArray());
        }

        /// <summary>
        /// Returns a value indicating whether a specified substring occurs within this string.
        /// </summary>
        /// <param name="target">The string to check.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparisonType">The type of <see cref="StringComparison"/> to use to locate the <paramref name="value"/> in the <paramref name="target"/>.</param>
        /// <returns>true if the value parameter occurs within this string, otherwise, false.</returns>
        public static bool Contains(this string target, string value, StringComparison comparisonType)
        {
            if (target == null || value == null)
                return false;

            return target.IndexOf(value, comparisonType) >= 0;
        }

        public static T ToEnum<T>(this string value) where T : struct, Enum => value.ToEnum(default(T));

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct, Enum
        {
            if (Enum.TryParse(value, true, out T result))
                return result;

            return defaultValue;
        }

        public static string TrimToLower(this string value, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            ReadOnlySpan<char> span = value.AsSpan().Trim();
            Span<char> lowerSpan = stackalloc char[span.Length];
            span.ToLowerSlim(lowerSpan, culture);

            return lowerSpan.ToString();
        }

        public static string TrimToLowerInvariant(this string value)
            => TrimToLower(value, CultureInfo.InvariantCulture);
    }
}