using System;
using Microsoft.AspNetCore.Http;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Contains ASP.NET Core specific string extensions, primarily for performing operations on URLs.
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
		public static string ToAbsoluteUrl(this string relativeUrl, HttpRequest currentRequest, string schemeOverride = null, string hostOverride = null, int portOverride = 0)
		{
			PathString applicationPath = currentRequest.PathBase;

			PathString virtualApplicationPath = applicationPath != "/"
				? applicationPath
				: PathString.Empty;

			// Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
			string absoluteVirtualPath = relativeUrl.StartsWith(virtualApplicationPath, StringComparison.OrdinalIgnoreCase)
				? relativeUrl
				: virtualApplicationPath.Add(relativeUrl).Value;

			int? currentPort = currentRequest.Host.Port;
			string port = portOverride > 0 ? portOverride.ToString() : (currentPort.HasValue && currentPort.Value != 80 ? (":" + currentPort) : string.Empty);

			return string.Format("{0}://{1}{2}{3}", schemeOverride ?? currentRequest.Scheme, hostOverride ?? currentRequest.Host.Value, port, absoluteVirtualPath);
		}
	}
}