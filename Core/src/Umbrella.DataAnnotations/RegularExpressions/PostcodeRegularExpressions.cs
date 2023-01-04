﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Umbrella.DataAnnotations.RegularExpressions;

/// <summary>
/// Contains regular expressions for validating postcodes.
/// </summary>
public static class PostcodeRegularExpressions
{
	/// <summary>
	/// A regular expression used to validate UK postcodes.
	/// </summary>
	/*lang=regex*/
	public const string UKPostcodeRegexString = "^((([A-Pa-pR-UWYZr-uwyz](\\d([A-HJKSTUWa-hjkstuw]|\\d)?|[A-Ha-hK-Yk-y]\\d([AaBbEeHhMmNnPpRrVvWwXxYy]|\\d)?))\\s*(\\d[ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})?)|[Gg][Ii][Rr]\\s*0[Aa][Aa])$";

	/// <summary>
	/// A regular expression used to partially validate UK postcodes, i.e. just the first part.
	/// </summary>
	/*lang=regex*/
	public const string UKPartialPostcodeRegexString = @"^[a-z]{1,2}\d{1,2}.*$";

	/// <summary>
	/// A regular expression used to validate UK postcodes.
	/// </summary>
	public static readonly Regex UKPostcodeRegex = new Regex(UKPostcodeRegexString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	/// <summary>
	/// A regular expression used to partially validate UK postcodes, i.e. just the first part.
	/// </summary>
	public static readonly Regex UKPartialPostcodeRegex = new Regex(UKPartialPostcodeRegexString, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
}