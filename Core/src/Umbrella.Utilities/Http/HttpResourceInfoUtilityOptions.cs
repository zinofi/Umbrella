using System;

namespace Umbrella.Utilities.Http
{
	// TODO V3: Look into refactoring some of these options classes that involve caching and extract the common properties into a base class
	// for consistency and perhaps some kind of shared behaviour in the future.
	/// <summary>
	/// The options for the <see cref="HttpResourceInfoUtility" /> class, typically registered with DI as a singleton.
	/// </summary>
	public class HttpResourceInfoUtilityOptions
	{
		/// <summary>
		/// Gets or sets a value indicating whether caching is enabled. This will override any caching parameters
		/// on methods of the <see cref="HttpResourceInfoUtility"/> class. Defaults to <see langword="true" />.
		/// </summary>
		public bool CacheEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets the cache timeout. Defaults to 1 hour.
		/// </summary>
		public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromHours(1);

		/// <summary>
		/// Gets or sets a value indicating whether sliding expiration is enabled. Defaults to <see langword="true" />.
		/// </summary>
		public bool CacheSlidingExpiration { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether or not to cache items in memory or use whatever the chosen distributed
		/// caching mechanism is for the application. Defaults to <see langword="true" />.
		/// </summary>
		public bool UseMemoryCache { get; set; } = true;
	}
}