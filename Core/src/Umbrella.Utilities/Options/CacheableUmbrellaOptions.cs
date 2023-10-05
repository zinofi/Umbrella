using Microsoft.Extensions.Caching.Memory;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Options;

namespace Umbrella.Utilities.Options;

/// <summary>
/// An abstract class that serves as the base class for all Umbrella Options classes that have cacheable properties.
/// </summary>
public abstract class CacheableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether to override the global <see cref="HybridCacheOptions.CacheEnabled"/> property. However, this override is only used when that property is set to <see langword="true"/>.
	/// If it is set to <see langword="false"/> then this override has no effect. Defaults to <see langword="true" />.
	/// </summary>
	public virtual bool CacheEnabled { get; set; } = true;

	/// <summary>
	/// Gets or sets the cache timeout. Defaults to 1 hour.
	/// </summary>
	public virtual TimeSpan CacheTimeout { get; set; } = TimeSpan.FromHours(1);

	/// <summary>
	/// Gets or sets a value indicating whether sliding expiration is enabled. Defaults to <see langword="true" />.
	/// </summary>
	public virtual bool CacheSlidingExpiration { get; set; } = true;

	/// <summary>
	/// Gets or sets the cache mode. Defaults to <see cref="HybridCacheMode.Memory"/>.
	/// </summary>
	public virtual HybridCacheMode CacheMode { get; set; } = HybridCacheMode.Memory;

	/// <summary>
	/// Gets or sets the cache priority when caching items in memory. Defaults to <see cref="CacheItemPriority.Normal"/>.
	/// </summary>
	public virtual CacheItemPriority CachePriority { get; set; } = CacheItemPriority.Normal;

	/// <summary>
	/// Gets or sets a value indicating whether the <see cref="HybridCache" /> will throw an exception when an internal exception is encountered, e.g. if there was a
	/// connection error when dealing with a distributed cache which may be transient.
	/// Setting this to <see langword="false" /> will mask any such exceptions from callers although these exceptions will be logged for diagnostic purposes.
	/// Defaults to <see langword="true" />
	/// </summary>
	public virtual bool CacheThrowOnFailure { get; set; } = true;
}