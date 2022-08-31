// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Umbrella.Utilities.Constants;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods that operation on <see langword="string"/> instances.
/// </summary>
public static class StringExtensions
{
	private const string HtmlTagPattern = @"<.*?>";
	private const string EllipsisPattern = @"[\.]+$";

	private static readonly Regex _htmlTagPatternRegex = new(HtmlTagPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	private static readonly Regex _ellipsisPatternRegex = new(EllipsisPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	/// <summary>
	/// Removes zero-width whitespace characters from the specified <paramref name="value"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A new string with the preambles removed.</returns>
	public static string RemovePreambles(this string value) => value.Replace(StringEncodingConstants.PreambleString, null);

	/// <summary>
	/// Normalizes new line characters to ensure consistency between Windows and Unix platforms.
	/// All new lines will be represented as \r\n strings.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A new string with the new lines normalized.</returns>
	public static string NormalizeNewLines(this string value) => value.Replace("\r\n", "\n").Replace("\n", "\r\n");

	/// <summary>
	/// Normalizes new line characters in a HTML encoded string to ensure consistency between Windows and Unix platforms.
	/// All new lines will be represented as encoded \r\n strings.
	/// </summary>
	/// <param name="encodedValue">The encoded value.</param>
	/// <returns>A new HTML encoded string with the new lines normalized.</returns>
	public static string NormalizeHtmlEncodedNewLines(this string encodedValue) => encodedValue.Replace(StringEncodingConstants.HtmlEncodedCrLfToken, StringEncodingConstants.HtmlEncodedLfToken).Replace(StringEncodingConstants.HtmlEncodedLfToken, StringEncodingConstants.HtmlEncodedCrLfToken);

	/// <summary>
	/// Reduces the whitespace in the specified <paramref name="value"/> by replacing multiple whitespace
	/// characters with single whitespace characters.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A new string with reduced whitespace.</returns>
	public static string ReduceWhitespace(this string value)
	{
		var newString = new StringBuilder();

		bool previousIsWhitespace = false;

		for (int i = 0; i < value.Length; i++)
		{
			if (value[i] == ' ')
			{
				if (previousIsWhitespace)
				{
					continue;
				}

				previousIsWhitespace = true;
			}
			else
			{
				previousIsWhitespace = false;
			}

			_ = newString.Append(value[i]);
		}

		return newString.ToString();
	}

	/// <summary>
	/// Truncates the specified <paramref name="value"/> to the <paramref name="maxLength"/> inclusive.
	/// </summary>
	/// <param name="value">The text.</param>
	/// <param name="maxLength">The maximum length.</param>
	/// <param name="stripHtml">Specifies if the <paramref name="value"/> should be stripped of HTML.</param>
	/// <returns>The truncated <paramref name="value"/>.</returns>
	[Obsolete("Use Humanizer")]
	public static string Truncate(this string value, int maxLength, bool stripHtml = true)
	{
		Guard.IsNotNull(value, nameof(value));

		// Ensure we strip out HTML tags
		if (stripHtml && value.Length > 0)
		{
			value = value.StripHtml();

			if (string.IsNullOrEmpty(value))
				return value;
		}

		return value.Length < maxLength ? value : AppendEllipsis(value.Substring(0, maxLength - 3));
	}

	/// <summary>
	/// Truncates the specified <paramref name="value"/> to the nearest word.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="maxLength">The maximum length.</param>
	/// <param name="stripHtml">Specifies if the <paramref name="value"/> should be stripped of HTML.</param>
	/// <returns>The truncated <paramref name="value"/>.</returns>
	[Obsolete("Use Humanizer")]
	public static string TruncateAtWord(this string value, int maxLength, bool stripHtml = true)
	{
		Guard.IsNotNull(value, nameof(value));

		// Ensure we strip out HTML tags
		if (stripHtml && value.Length > 0)
			value = value.StripHtml();

		return value.Length < maxLength || value.IndexOf(" ", maxLength) == -1
			? value
			: AppendEllipsis(value.Substring(0, value.IndexOf(" ", maxLength)));
	}

	/// <summary>
	/// Strips HTML non-breaking space values from the <paramref name="value"/> and replaces them with empty strings.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The result or <see langword="null"/> if the <paramref name="value"/> provided was <see langword="null"/>.</returns>
	public static string StripNbsp(this string value)
	{
		Guard.IsNotNull(value, nameof(value));

		return value.Replace("&nbsp;", "");
	}

	/// <summary>
	/// Determines whether the specified <paramref name="value"/> has a length between the <paramref name="minLength"/> and <paramref name="maxLength"/> inclusive.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="minLength">The minimum length.</param>
	/// <param name="maxLength">The maximum length.</param>
	/// <param name="allowNull">if set to <see langword="true"/> and the specified <paramref name="value"/> is <see langword="null"/> or whitespace, the return value will be <see langword="true"/>.</param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="value"/> is between <paramref name="minLength"/> and <paramref name="maxLength"/> inclusive,
	///   or <see langword="true"/> if the <paramref name="value"/> is <see langword="null"/> or whitespace and <paramref name="allowNull"/> is <see langword="true"/>.
	///   All other conditions will return <see langword="false"/>.
	/// </returns>
	public static bool IsValidLength(this string? value, int minLength, int maxLength, bool allowNull = true) => string.IsNullOrWhiteSpace(value) ? allowNull : value?.Length >= minLength && value.Length <= maxLength;

	/// <summary>
	/// Strips HTML tags from the specified <paramref name="value"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The value with all HTML tags removed.</returns>
	[Obsolete("Use the dotnet toolkit when released.")]
	public static string StripHtml(this string value)
	{
		Guard.IsNotNull(value, nameof(value));

		return _htmlTagPatternRegex.Replace(value, string.Empty);
	}

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to CamelCase using the conversion rules of the
	/// specified <paramref name="cultureInfo"/> or <see cref="CultureInfo.CurrentCulture"/> if not specified.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="cultureInfo">The culture used for the conversion. If not specified <see cref="CultureInfo.CurrentCulture"/> is used.</param>
	/// <returns>The <paramref name="value"/> converted to CamelCase.</returns>
	[Obsolete("Use Humanizer")]
	public static string ToCamelCase(this string value, CultureInfo? cultureInfo = null) => ToCamelCaseInternal(value, cultureInfo ?? CultureInfo.CurrentCulture);

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to CamelCase using the conversion rules of the
	/// invariant culture.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The <paramref name="value"/> converted to CamelCase.</returns>
	[Obsolete("Use Humanizer")]
	public static string ToCamelCaseInvariant(this string value) => ToCamelCaseInternal(value, CultureInfo.InvariantCulture);

	private static string ToCamelCaseInternal(string value, CultureInfo cultureInfo)
	{
		Guard.IsNotNull(value, nameof(value));
		Guard.IsNotNull(cultureInfo, nameof(cultureInfo));

		if (value.Length is 1)
			return value.ToLower(cultureInfo);

		// If 1st char is already in lowercase, return the value untouched
		if (char.IsLower(value[0]))
			return value;

		Span<char> buffer = value.Length <= StackAllocConstants.MaxCharSize
			? stackalloc char[value.Length]
			: new char[value.Length];

		bool stop = false;

		for (int i = 0; i < value.Length; i++)
		{
			if (!stop)
			{
				if (char.IsUpper(value[i]))
				{
					if (i > 1 && char.IsLower(value[i - 1]))
					{
						stop = true;
					}
					else
					{
						buffer[i] = char.ToLower(value[i], cultureInfo);
						continue;
					}
				}
				else if (i > 1)
				{
					// Encountered first lowercase char
					// Check previous char and see if that was uppercase before we made it lowercase
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

		return buffer.ToString();
	}

	/// <summary>
	/// Appends an ellipsis to the specified <paramref name="value"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The value with an ellipsis appended.</returns>
	[Obsolete("Use Humanizer")]
	public static string AppendEllipsis(string value)
	{
		Guard.IsNotNull(value, nameof(value));

		value += "...";

		return _ellipsisPatternRegex.Replace(value, "...");
	}

	/// <summary>
	/// Converts the any HTML br tags to line breaks.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The converted value.</returns>
	public static string ConvertHtmlBrTagsToLineBreaks(this string value)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		var sb = new StringBuilder(value);

		_ = sb.ConvertHtmlBrTagsToReplacement(Environment.NewLine);

		return sb.ToString();
	}

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to SnakeCase using the conversion rules of the
	/// specified <paramref name="cultureInfo"/> or <see cref="CultureInfo.CurrentCulture"/> if not specified.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="lowerCase">if set to <see langword="true"/> converts the output to lowercase.</param>
	/// <param name="removeWhiteSpace">if set to <see langword="true"/> removes all whitespace.</param>
	/// <param name="cultureInfo">The culture used for the conversion. If not specified <see cref="CultureInfo.CurrentCulture"/> is used.</param>
	/// <returns>The <paramref name="value"/> converted to SnakeCase.</returns>
	[Obsolete("Use Humanizer")]
	public static string ToSnakeCase(this string value, bool lowerCase = true, bool removeWhiteSpace = true, CultureInfo? cultureInfo = null)
		=> ToSnakeCaseInternal(value, lowerCase, removeWhiteSpace, cultureInfo ?? CultureInfo.CurrentCulture);

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to SnakeCase using the conversion rules of the
	/// invariant culture.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="lowerCase">if set to <see langword="true"/> converts the output to lowercase.</param>
	/// <param name="removeWhiteSpace">if set to <see langword="true"/> removes all whitespace.</param>
	/// <returns>The <paramref name="value"/> converted to SnakeCase.</returns>
	[Obsolete("Use Humanizer")]
	public static string ToSnakeCaseInvariant(this string value, bool lowerCase = true, bool removeWhiteSpace = true)
		=> ToSnakeCaseInternal(value, lowerCase, removeWhiteSpace, CultureInfo.InvariantCulture);

	private static string ToSnakeCaseInternal(string value, bool lowerCase, bool removeWhiteSpace, CultureInfo cultureInfo)
	{
		if (string.IsNullOrWhiteSpace(value))
			return value;

		if (value.Length is 1)
			return lowerCase ? value.ToLower(cultureInfo) : value;

		var buffer = new List<char>(value.Length)
		{
			lowerCase ? char.ToLower(value[0], cultureInfo) : value[0]
		};

		for (int i = 1; i < value.Length; i++)
		{
			char current = value[i];

			if (removeWhiteSpace && char.IsWhiteSpace(current))
				continue;

			if (char.IsUpper(value[i]))
			{
				if (lowerCase)
					current = char.ToLower(current, cultureInfo);

				buffer.Add('_');
			}

			buffer.Add(current);
		}

		return new string(buffer.ToArray());
	}

	/// <summary>
	/// Returns a value indicating whether a specified substring occurs within this string.
	/// </summary>
	/// <param name="target">The string to check.</param>
	/// <param name="value">The string to seek.</param>
	/// <param name="comparisonType">The type of <see cref="StringComparison"/> to use to locate the <paramref name="value"/> in the <paramref name="target"/>.</param>
	/// <returns>true if the value parameter occurs within this string, otherwise, false.</returns>
	public static bool Contains(this string? target, string? value, StringComparison comparisonType) => target is not null && value is not null && target.IndexOf(value, comparisonType) >= 0;

	/// <summary>
	/// Converts the specified <paramref name="value"/> to the enum type <typeparamref name="T"/>. If the conversion fails,
	/// the <paramref name="defaultValue"/> is returned.
	/// </summary>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <param name="value">The value.</param>
	/// <param name="defaultValue">The default value.</param>
	/// <returns>The enum value or the default value.</returns>
	public static T ToEnum<T>(this string? value, T defaultValue = default) where T : struct, Enum => Enum.TryParse(value, true, out T result) ? result : defaultValue;

	/// <summary>
	/// Trims the specified <paramref name="value"/> and converts it to lowercase using the specified <paramref name="culture"/>.
	/// If the<paramref name= "culture" /> is <see langword= "null" />, <see cref="CultureInfo.CurrentCulture" /> is used.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="culture">The optional culture. If this is <see langword="null" />, <see cref="CultureInfo.CurrentCulture"/> is used.</param>
	/// <returns>The converted string or <see langword="null"/> if the <paramref name="value"/> was null.</returns>
	public static string TrimToLower(this string value, CultureInfo? culture = null)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		ReadOnlySpan<char> span = value.AsSpan().Trim();
		Span<char> lowerSpan = span.Length <= StackAllocConstants.MaxCharSize ? stackalloc char[span.Length] : new char[span.Length];
		span.ToLowerSlim(lowerSpan, culture ?? CultureInfo.CurrentCulture);

		return lowerSpan.ToString();
	}

	/// <summary>
	/// Trims the specified <paramref name="value"/> and converts it to lowercase using the invariant culture.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The converted string or <see langword="null"/> if the <paramref name="value"/> was null.</returns>
	public static string TrimToLowerInvariant(this string value)
		=> TrimToLower(value, CultureInfo.InvariantCulture);

	/// <summary>
	/// Trims the specified <paramref name="value"/> and converts it to uppercase using the specified <paramref name="culture"/>.
	/// If the <paramref name="culture"/> is <see langword="null" />, <see cref="CultureInfo.CurrentCulture"/> is used.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="culture">The optional culture. If this is <see langword="null" />, <see cref="CultureInfo.CurrentCulture"/> is used.</param>
	/// <returns>The converted string or <see langword="null"/> if the <paramref name="value"/> was null.</returns>
	public static string TrimToUpper(this string value, CultureInfo? culture = null)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		ReadOnlySpan<char> span = value.AsSpan().Trim();
		Span<char> upperSpan = span.Length <= StackAllocConstants.MaxCharSize ? stackalloc char[span.Length] : new char[span.Length];
		span.ToUpperSlim(upperSpan, culture!);

		return upperSpan.ToString();
	}

	/// <summary>
	/// Trims the specified <paramref name="value"/> and converts it to uppercase using the invariant culture.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The converted string or <see langword="null"/> if the <paramref name="value"/> was null.</returns>
	public static string TrimToUpperInvariant(this string value)
		=> TrimToUpper(value, CultureInfo.InvariantCulture);

	/// <summary>
	/// Attempts to convert the name of a person into some kind of normalized format
	/// where the name is not presently in a reasonable format, e.g. riCHARd would be better transformed
	/// into Richard. This method only deals with the simplest of cases currently.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <returns>The normalized name</returns>
	public static string? ToPersonNameCase(this string? name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return name;

		Span<char> span = name!.Length <= StackAllocConstants.MaxCharSize ? stackalloc char[name.Length] : new char[name.Length];

		for (int i = 0; i < name.Length; i++)
		{
			char currentChar = name[i];

			// First letter should always be uppercase
			if (i == 0)
			{
				span[i] = char.ToUpper(currentChar);
				continue;
			}

			span[i] = char.ToLower(currentChar);

			if (currentChar == '-' && i < name.Length - 1)
			{
				// Ensure the next letter is in uppercase as we are most likely dealing with a double barrelled name
				span[i + 1] = char.ToUpper(name[i + 1]);
				i++;
				continue;
			}
		}

		return span.ToString();
	}

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to title case using the rules of the specified
	/// <paramref name="cultureInfo"/>. If <paramref name="cultureInfo"/> is <see langword="null"/>, it defaults
	/// to <see cref="CultureInfo.CurrentCulture"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="cultureInfo">The optional culture information. Defaults to <see cref="CultureInfo.CurrentCulture"/>.</param>
	/// <returns>The <paramref name="value"/> converted to title case.</returns>
	[Obsolete("Use Humanizer")]
	public static string? ToTitleCase(this string? value, CultureInfo? cultureInfo = null)
		=> string.IsNullOrWhiteSpace(value) ? value : (cultureInfo ?? CultureInfo.CurrentCulture).TextInfo.ToTitleCase(value);

	/// <summary>
	/// Converts to the specified <paramref name="value"/> to title case using the rules of the invariant culture.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The <paramref name="value"/> converted to title case.</returns>
	[Obsolete("Use Humanizer")]
	public static string? ToTitleCaseInvariant(this string? value)
		=> ToTitleCase(value, CultureInfo.InvariantCulture);
}