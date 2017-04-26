using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.WebUtilities.Http
{
    public class HttpHeaderValueUtility : IHttpHeaderValueUtility
    {
        public string CreateLastModifiedHeaderValue(DateTimeOffset lastModified)
            => lastModified.UtcDateTime.ToString("r");

        public string CreateETagHeaderValue(DateTimeOffset lastModified, long contentLength)
        {
            long eTagHash = lastModified.UtcDateTime.ToFileTimeUtc() ^ contentLength;
            string eTagValue = Convert.ToString(eTagHash, 16);

            return $"\"{eTagValue}\"";
        }
    }
}