using System;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Options;

namespace Umbrella.Utilities.Abstractions
{
	/// <summary>
	/// An abstract class that serves as the base class for all Umbrella Options classes that have cacheable properties.
	/// </summary>
	public abstract class CacheableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets a value indicating whether caching is enabled. This will override any caching parameters
		/// on methods of the consuming class. Defaults to <see langword="true" />.
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
		public virtual HybridCacheMode CacheMode { get; set; }

		/// <summary>
		/// Gets or sets the cache priority when caching items in memory. Defaults to <see cref="CacheItemPriority.Normal"/>.
		/// </summary>
		public virtual CacheItemPriority CachePriority { get; set; } = CacheItemPriority.Normal;

		/// <summary>
		/// Gets or sets a value indicating whether to override the global <see cref="HybridCacheOptions.CacheEnabled"/> property. However, this override is only used when that property is set to <see langword="true"/>.
		/// If it is set to <see langword="false"/> then this override has no effect.
		/// </summary>
		public virtual bool CacheThrowOnFailure { get; set; } = true;
	}
}