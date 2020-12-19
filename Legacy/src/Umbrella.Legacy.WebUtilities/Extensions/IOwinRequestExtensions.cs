using System;
using System.Linq;
using Microsoft.Owin;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IOwinRequest"/>.
	/// </summary>
	public static class IOwinRequestExtensions
	{
		/// <summary>
		/// Determines if the specified <paramref name="valueToMatch"/> matches the value of the "If-Modified-Since" request header.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="valueToMatch">The value to match.</param>
		/// <returns><see langword="true"/> if it matches; otherwise <see langword="false"/>.</returns>
		public static bool IfModifiedSinceHeaderMatched(this IOwinRequest request, DateTimeOffset valueToMatch)
		{
			Guard.ArgumentNotNull(request, nameof(request));

			string ifModifiedSince = request.Headers["If-Modified-Since"];

			if (!string.IsNullOrWhiteSpace(ifModifiedSince))
			{
				DateTime lastModified = DateTime.Parse(ifModifiedSince).ToUniversalTime();

				return lastModified == valueToMatch;
			}

			return false;
		}

		/// <summary>
		/// Determines if the specified <paramref name="valueToMatch"/> matches the value of the "If-None-Match" request header.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="valueToMatch">The value to match.</param>
		/// <returns><see langword="true"/> if it matches; otherwise <see langword="false"/>.</returns>
		public static bool IfNoneMatchHeaderMatched(this IOwinRequest request, string valueToMatch)
		{
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNullOrWhiteSpace(valueToMatch, nameof(valueToMatch));

			string ifNoneMatch = request.Headers["If-None-Match"];

			return !string.IsNullOrWhiteSpace(ifNoneMatch) && string.Compare(ifNoneMatch, valueToMatch, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Determines if the requesting client supports rendering images encoded using the image/webp format.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><see langword="true"/> if it does; otherwise <see langword="false"/>.</returns>
		public static bool AcceptsWebP(this IOwinRequest request)
			=> request.Headers.TryGetValue("Accept", out string[] values) && values.Any(x => !string.IsNullOrEmpty(x) && x.Contains("image/webp", StringComparison.OrdinalIgnoreCase));

		/// <summary>
		/// Determines whether the requesting client is IE by checking the User-Agent header to see if it contains
		/// the strings "MSIE" or "Trident" using ordinal case-insensitive comparison rules.
		/// </summary>
		/// <param name="request">The request.</param>
		public static bool IsInternetExplorer(this IOwinRequest request)
		{
			string userAgent = request.Headers["User-Agent"];

			if (string.IsNullOrWhiteSpace(userAgent))
				return false;

			return userAgent.Contains("MSIE", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Trident", StringComparison.OrdinalIgnoreCase);
		}
	}
}