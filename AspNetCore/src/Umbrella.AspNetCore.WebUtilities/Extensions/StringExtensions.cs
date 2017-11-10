using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the provided app-relative path into an absolute Url containing the 
        /// full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo or /path/to/foo to http://www.web.com/path/to/foo</example>
        public static string ToAbsoluteUrl(this string relativeUrl, HttpRequest request, string scheme = "http")
        {
            Guard.ArgumentNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));
            Guard.ArgumentNotNull(request, nameof(request));
            Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));

            string cleanedPath = relativeUrl.StartsWith("~")
                    ? relativeUrl.Trim().Remove(0, 1)
                    : relativeUrl.Trim();

            return $"{scheme}://{request.Host}{cleanedPath}";
        }
    }
}
