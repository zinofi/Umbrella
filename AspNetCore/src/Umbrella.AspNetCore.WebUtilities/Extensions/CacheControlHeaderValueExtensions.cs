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
		/// Sets the correct caching properties on the specified <see cref="CacheControlHeaderValue"/> using the specified <see cref="MiddlewareHttpCacheability"/> value.
		/// </summary>
		/// <param name="cacheControlHeaderValue">The cache control header value.</param>
		/// <param name="httpCacheability">The HTTP cacheability.</param>
		public static void SetFrontEndCompressionMiddlewareHttpCacheability(this CacheControlHeaderValue cacheControlHeaderValue, MiddlewareHttpCacheability httpCacheability)
		{
			switch (httpCacheability)
			{
				case MiddlewareHttpCacheability.NoCache:
					cacheControlHeaderValue.NoCache = true;
					break;
				case MiddlewareHttpCacheability.NoStore:
					cacheControlHeaderValue.NoStore = true;
					break;
				case MiddlewareHttpCacheability.Private:
					cacheControlHeaderValue.Private = true;
					break;
				case MiddlewareHttpCacheability.Public:
					cacheControlHeaderValue.Public = true;
					break;
			}
		}
	}
}