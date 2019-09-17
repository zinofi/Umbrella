using System;
using Microsoft.Extensions.Caching.Memory;

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
		/// Gets or sets a value indicating whether or not to cache items in memory or use whatever the chosen distributed
		/// caching mechanism is for the application. Defaults to <see langword="true" />.
		/// </summary>
		public virtual bool UseMemoryCache { get; set; } = true;

		/// <summary>
		/// Gets or sets the cache priority when caching items in memory. Defaults to <see cref="CacheItemPriority.Normal"/>.
		/// </summary>
		public virtual CacheItemPriority MemoryCachePriority { get; set; } = CacheItemPriority.Normal;
	}
}