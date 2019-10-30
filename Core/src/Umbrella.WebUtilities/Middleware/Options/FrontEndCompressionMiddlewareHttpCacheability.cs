using System;

namespace Umbrella.WebUtilities.Middleware.Options
{
	public enum FrontEndCompressionMiddlewareHttpCacheability
	{
		Public,
		Private,
		NoCache,
		NoStore
	}

	public static class FrontEndCompressionMiddlewareHttpCacheabilityExtensions
	{
		public static string ToCacheControlString(this FrontEndCompressionMiddlewareHttpCacheability value)
		{
			switch (value)
			{
				case FrontEndCompressionMiddlewareHttpCacheability.NoCache:
					return "no-cache";
				case FrontEndCompressionMiddlewareHttpCacheability.NoStore:
					return "no-store";
				case FrontEndCompressionMiddlewareHttpCacheability.Private:
					return "private";
				case FrontEndCompressionMiddlewareHttpCacheability.Public:
					return "public";
			}

			throw new NotSupportedException($"The specified value: {value} is not supported.");
		}
	}
}