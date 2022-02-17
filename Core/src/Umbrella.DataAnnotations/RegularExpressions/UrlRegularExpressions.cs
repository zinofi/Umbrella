// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Umbrella.DataAnnotations.RegularExpressions
{
	/// <summary>
	/// Contains regular expressions for validating URLs.
	/// </summary>
	public static class UrlRegularExpressions
	{
		/// <summary>
		/// A regular expression used to validate URLs.
		/// </summary>
		public const string UrlRegexString = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

		/// <summary>
		/// A regular expression used to validate URLs.
		/// </summary>
		public static Regex UrlRegex = new Regex(UrlRegexString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}