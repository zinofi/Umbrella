using Microsoft.Net.Http.Headers;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="CacheControlHeaderValue"/> type.
	/// </summary>
	public static class CacheControlHeaderValueExtensions
	{
		/// <summary>
		/// Sets the correct caching properties on the specified <see cref="CacheControlHeaderValue"/> using the specified <see cref="FrontEndCompressionMiddlewareHttpCacheability"/> value.
		/// </summary>
		/// <param name="cacheControlHeaderValue">The cache control header value.</param>
		/// <param name="httpCacheability">The HTTP cacheability.</param>
		public static void SetFrontEndCompressionMiddlewareHttpCacheability(this CacheControlHeaderValue cacheControlHeaderValue, FrontEndCompressionMiddlewareHttpCacheability httpCacheability)
		{
			switch (httpCacheability)
			{
				case FrontEndCompressionMiddlewareHttpCacheability.NoCache:
					cacheControlHeaderValue.NoCache = true;
					break;
				case FrontEndCompressionMiddlewareHttpCacheability.NoStore:
					cacheControlHeaderValue.NoStore = true;
					break;
				case FrontEndCompressionMiddlewareHttpCacheability.Private:
					cacheControlHeaderValue.Private = true;
					break;
				case FrontEndCompressionMiddlewareHttpCacheability.Public:
					cacheControlHeaderValue.Public = true;
					break;
			}
		}
	}
}