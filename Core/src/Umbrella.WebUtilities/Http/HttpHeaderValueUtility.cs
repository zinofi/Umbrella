using System;
using Umbrella.WebUtilities.Http.Abstractions;

namespace Umbrella.WebUtilities.Http;

internal class HttpHeaderValueUtility : IHttpHeaderValueUtility
{
	#region IHttpHeaderValueUtility Members
	/// <inheritdoc />
	public string CreateLastModifiedHeaderValue(DateTimeOffset lastModified)
		=> lastModified.UtcDateTime.ToString("r");

	/// <inheritdoc />
	public string CreateETagHeaderValue(DateTimeOffset lastModified, long contentLength)
	{
		long eTagHash = lastModified.UtcDateTime.ToFileTimeUtc() ^ contentLength;
		string eTagValue = Convert.ToString(eTagHash, 16);

		return $"\"{eTagValue}\"";
	}
	#endregion
}