using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Owin
{
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
    }
}