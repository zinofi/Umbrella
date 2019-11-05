using Microsoft.Net.Http.Headers;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	public static class CacheControlHeaderValueExtensions
	{
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