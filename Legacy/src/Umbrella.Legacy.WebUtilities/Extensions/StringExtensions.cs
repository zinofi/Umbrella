using System;
using System.Web;
using Umbrella.Utilities;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
	/// <summary>
	/// Contains ASP.NET specific string extensions, primarily for performing operations on URLs.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Converts the provided app-relative path into an absolute Url containing the full host name.
		/// </summary>
		/// <param name="relativeUrl">The relative URL.</param>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="schemeOverride">The scheme override.</param>
		/// <param name="hostOverride">The host override.</param>
		/// <param name="portOverride">The port override.</param>
		/// <returns>Provided relativeUrl parameter as fully qualified Url.</returns>
		/// <example>~/path/to/foo or /path/to/foo to http://www.web.com/path/to/foo</example>
		public static string ToAbsoluteUrl(this string relativeUrl, Uri requestUri, string schemeOverride = null, string hostOverride = null, int portOverride = 0)
		{
			Guard.ArgumentNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));
			Guard.ArgumentNotNull(requestUri, nameof(requestUri));

			if (relativeUrl.StartsWith("/"))
				relativeUrl = relativeUrl.Insert(0, "~");
			if (!relativeUrl.StartsWith("~/"))
				relativeUrl = relativeUrl.Insert(0, "~/");

			string port = portOverride > 0 ? portOverride.ToString() : (requestUri.Port != 80 ? (":" + requestUri.Port) : string.Empty);

			return string.Format("{0}://{1}{2}{3}", schemeOverride ?? requestUri.Scheme, hostOverride ?? requestUri.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
		}
	}
}