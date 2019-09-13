using System;

namespace Umbrella.Utilities.Caching.Options
{
	public delegate string HybridCacheKeyBuilder(Type type, string key);

	public class HybridCacheOptions
	{
		public bool CacheEnabled { get; set; }
		public HybridCacheKeyBuilder CacheKeyBuilder { get; set; }
		public TimeSpan MaxCacheTimeout { get; set; } = TimeSpan.FromDays(3650); // Arbitrary default of ~10 years.
		public HybridCacheAnalyticsMode AnalyticsMode { get; set; }
	}
}