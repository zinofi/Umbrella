using System;

namespace Umbrella.WebUtilities.Http.Abstractions
{
	/// <summary>
	/// A utility used to create common HTTP Header values.
	/// </summary>
	public interface IHttpHeaderValueUtility
    {
		/// <summary>
		/// Creates the ETag header value.
		/// </summary>
		/// <param name="lastModified">The last modified date.</param>
		/// <param name="contentLength">Length of the content.</param>
		/// <returns>The value for use with the header.</returns>
		string CreateETagHeaderValue(DateTimeOffset lastModified, long contentLength);

		/// <summary>
		/// Creates the last modified header value.
		/// </summary>
		/// <param name="lastModified">The last modified date.</param>
		/// <returns>The value for use with the header.</returns>
		string CreateLastModifiedHeaderValue(DateTimeOffset lastModified);
    }
}