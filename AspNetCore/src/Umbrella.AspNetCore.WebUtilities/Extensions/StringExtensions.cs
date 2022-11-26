// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Umbrella.Utilities.Constants;
using Umbrella.WebUtilities.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Extensions;

/// <summary>
/// Contains ASP.NET Core specific string extensions.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Converts the provided app-relative path into an absolute Url containing the full host name.
	/// </summary>
	/// <param name="relativeUrl">The relative URL.</param>
	/// <param name="currentRequest">The current request.</param>
	/// <param name="schemeOverride">The scheme override.</param>
	/// <param name="hostOverride">The host override.</param>
	/// <param name="portOverride">The port override.</param>
	/// <returns>Provided relativeUrl parameter as fully qualified Url.</returns>
	/// <example>~/path/to/foo or /path/to/foo to http://www.web.com/path/to/foo</example>
	public static string ToAbsoluteUrl(this string relativeUrl, HttpRequest currentRequest, string? schemeOverride = null, string? hostOverride = null, int? portOverride = null)
	{
		PathString applicationPath = currentRequest.PathBase;

		// Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
		string? absoluteVirtualPath = applicationPath.HasValue && relativeUrl.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase)
			? relativeUrl
			: applicationPath.Add(relativeUrl).Value;

		if (absoluteVirtualPath is null)
		{
			var exception = new UmbrellaWebException("The absoluteVirtualPath could not be determined.");
			exception.Data.Add(nameof(relativeUrl), relativeUrl);
			exception.Data.Add(nameof(applicationPath), applicationPath);

			throw exception;
		}

		int? currentPort = currentRequest.Host.Port;
		string? port = portOverride > 0 ? portOverride.ToString() : (currentPort.HasValue && currentPort.Value != 80 ? (":" + currentPort) : string.Empty);

		return string.Format("{0}://{1}{2}{3}", schemeOverride ?? currentRequest.Scheme, hostOverride ?? currentRequest.Host.Value, port, absoluteVirtualPath);
	}

	/// <summary>
	/// Encodes the specified <paramref name="value"/> as HTML and then replaces all encoded new line characters with the specified <paramref name="replacement"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="replacement">The replacement.</param>
	/// <returns>The HTML encoded output.</returns>
	public static IHtmlContent? ReplaceNewLine(this string? value, string replacement = "<br />")
		=> string.IsNullOrWhiteSpace(value) ? null : (IHtmlContent)new HtmlString(HtmlEncoder.Default.Encode(value).NormalizeHtmlEncodedNewLines().Replace(StringEncodingConstants.HtmlEncodedCrLfToken, replacement));
}