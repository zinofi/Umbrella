﻿namespace Umbrella.Utilities.Caching.Options;

/// <summary>
/// A delegate for specifiying the optional cache key builder.
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
	/// Defaults to <see langword="false" />.
	/// </summary>
	public bool CacheEnabled { get; set; }

	/// <summary>
	/// Gets or sets the cache key builder. This is only really needed when you want the cache key to have some kind of common element to the key, e.g. a prefix
	/// so that items that have gone into the Memory or Distributed cache have done so via the <see cref="HybridCache"/>.
	/// </summary>
	public HybridCacheKeyBuilder? CacheKeyBuilder { get; set; }

	/// <summary>
	/// Gets or sets the default cache timeout in the event that callers of methods on the <see cref="HybridCache"/> do not provide a cache timeout value.
	/// Defaults to 3650 days (10 years) to have the result of the cache never clearing in practice.
	/// </summary>
	public TimeSpan DefaultCacheTimeout { get; set; } = TimeSpan.FromDays(3650); // Arbitrary default of ~10 years.

	/// <summary>
	/// Gets or sets the analytics mode. By default, analytics is disabled for performance reasons as it incurs a small overhead.
	/// </summary>
	public HybridCacheAnalyticsMode AnalyticsMode { get; set; }
}