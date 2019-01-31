using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IOwinRequest"/>.
    /// </summary>
    public static class IOwinRequestExtensions
    {
        public static bool IfModifiedSinceHeaderMatched(this IOwinRequest request, DateTimeOffset valueToMatch)
        {
            string ifModifiedSince = request.Headers["If-Modified-Since"];
            if (!string.IsNullOrWhiteSpace(ifModifiedSince))
            {
                DateTime lastModified = DateTime.Parse(ifModifiedSince).ToUniversalTime();

                return lastModified == valueToMatch;
            }

            return false;
        }

        public static bool IfNoneMatchHeaderMatched(this IOwinRequest request, string valueToMatch)
        {
            string ifNoneMatch = request.Headers["If-None-Match"];
            if (!string.IsNullOrWhiteSpace(ifNoneMatch))
            {
                return string.Compare(ifNoneMatch, valueToMatch, StringComparison.OrdinalIgnoreCase) == 0;
            }

            return false;
        }

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