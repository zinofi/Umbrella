using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="HttpRequest"/> type.
	/// </summary>
	public static class HttpRequestExtensions
	{
		public static bool IfModifiedSinceHeaderMatched(this HttpRequest request, DateTimeOffset valueToMatch)
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

		public static bool IfNoneMatchHeaderMatched(this HttpRequest request, string valueToMatch)
		{
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNullOrWhiteSpace(valueToMatch, nameof(valueToMatch));

			string ifNoneMatch = request.Headers["If-None-Match"];

			return !string.IsNullOrWhiteSpace(ifNoneMatch)
				? string.Compare(ifNoneMatch, valueToMatch, StringComparison.OrdinalIgnoreCase) == 0
				: false;
		}

		public static bool AcceptsWebP(this HttpRequest request) => request.Headers.TryGetValue("Accept", out StringValues values)
				? values.Any(x => !string.IsNullOrEmpty(x) && x.Contains("image/webp", StringComparison.OrdinalIgnoreCase))
				: false;

		/// <summary>
		/// Determines whether the requesting client is IE by checking the User-Agent header to see if it contains
		/// the strings "MSIE" or "Trident" using ordinal case-insensitive comparison rules.
		/// </summary>
		/// <param name="request">The request.</param>
		public static bool IsInternetExplorer(this HttpRequest request)
		{
			string userAgent = request.Headers["User-Agent"];

			if (string.IsNullOrWhiteSpace(userAgent))
				return false;

			return userAgent.Contains("MSIE", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Trident", StringComparison.OrdinalIgnoreCase);
		}
	}
}