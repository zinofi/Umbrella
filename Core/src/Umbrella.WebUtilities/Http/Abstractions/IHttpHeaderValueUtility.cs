using System;

namespace Umbrella.WebUtilities.Http.Abstractions
{
    public interface IHttpHeaderValueUtility
    {
        string CreateETagHeaderValue(DateTimeOffset lastModified, long contentLength);
        string CreateLastModifiedHeaderValue(DateTimeOffset lastModified);
    }
}