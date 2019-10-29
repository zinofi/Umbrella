using System;

namespace Umbrella.Utilities.Caching.Options
{
	/// <summary>
	/// A delegate for specifiying the cache key builder.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="key">The key.</param>
	/// <returns>The built cache key.</returns>
	public delegate string HybridCacheKeyBuilder(Type type, string key);

	/// <summary>
	/// The options for the <see cref="HybridCache"/>.
	/// </summary>
	public class HybridCacheOptions
	{
		/// <summary>
		/// Gets or sets a value indicating whether caching is enabled. This value can be overridden by the individual methods of the <see cref="HybridCache"/>.
		/// However, if this property is set to <see langword="false"/>, caching will be disabled globally and the overrides will have no effect.
		/// </summary>
		public bool CacheEnabled { get; set; }

		/// <summary>
		/// Gets or sets the cache key builder.
		/// </summary>
		public HybridCacheKeyBuilder CacheKeyBuilder { get; set; }

		/// <summary>
		/// Gets or sets the default cache timeout in the event that callers of methods on the <see cref="HybridCache"/> do not provide a cache timeout value.
		/// </summary>
		public TimeSpan DefaultCacheTimeout { get; set; } = TimeSpan.FromDays(3650); // Arbitrary default of ~10 years.

		/// <summary>
		/// Gets or sets the analytics mode. By default, analytics is disabled for performance reasons as it incurs a small overhead.
		/// </summary>
		public HybridCacheAnalyticsMode AnalyticsMode { get; set; }
	}
}