using System;

namespace Umbrella.WebUtilities.Http
{
    public interface IHttpHeaderValueUtility
    {
        string CreateETagHeaderValue(DateTimeOffset lastModified, long contentLength);
        string CreateLastModifiedHeaderValue(DateTimeOffset lastModified);
    }
}