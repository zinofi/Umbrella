// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Umbrella.DataAnnotations.RegularExpressions;

/// <summary>
/// Contains regular expressions for validating phone numbers.
/// </summary>
public static class PhoneRegularExpressions
{
	/// <summary>
	/// A regular expression used to validate UK mobile numbers.
	/// </summary>
	public const string UKMobileRegexString = @"^07\d{3}\s?\d{6}$";

	/// <summary>
	/// A regular expression used to validate UK phone numbers.
	/// </summary>
	public const string UKPhoneRegexString = @"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$";

	/// <summary>
	/// A regular expression used to validate UK mobile numbers.
	/// </summary>
	public static Regex UKMobileRegex = new Regex(UKMobileRegexString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	/// <summary>
	/// A regular expression used to validate UK phone numbers.
	/// </summary>
	public static Regex UKPhoneRegex = new Regex(UKPhoneRegexString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
}