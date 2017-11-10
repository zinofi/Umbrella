using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Http
{
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

            if (!string.IsNullOrWhiteSpace(ifNoneMatch))
                return string.Compare(ifNoneMatch, valueToMatch, StringComparison.InvariantCultureIgnoreCase) == 0;

            return false;
        }
    }
}