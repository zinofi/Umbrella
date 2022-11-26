namespace Umbrella.Utilities.Caching;

/// <summary>
/// The analytics mode for the <see cref="HybridCache"/>.
/// </summary>
public enum HybridCacheAnalyticsMode
{
	/// <summary>
	/// Disables analytics.
	/// </summary>
	None = 0,

	/// <summary>
	/// Enables tracking of cache keys only.
	/// </summary>
	TrackKeys = 1,

	/// <summary>
	/// Enables tracking of both cache keys and a count of cache hits for each key.
	/// </summary>
	TrackKeysAndHits = 3
}