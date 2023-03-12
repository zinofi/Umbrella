namespace Umbrella.Utilities.Caching;

/// <summary>
/// Represents the meta-data of an entry stored in the <see cref="HybridCache"/> when the cached item is stored in-memory.
/// No such entries are created when items are stored in a distributed cache as they cannot be reliably tracked.
/// </summary>
public class HybridCacheMetaEntry
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HybridCacheMetaEntry"/> class.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="timeout">The timeout.</param>
	/// <param name="isSlidingExpiration">Specifies if the entry has a sliding expiration value.</param>
	public HybridCacheMetaEntry(string key, in TimeSpan timeout, bool isSlidingExpiration)
	{
		Key = key;
		Timeout = timeout;
		IsSlidingExpiration = isSlidingExpiration;
		CreatedDate = DateTime.UtcNow;
	}

	/// <summary>
	/// Gets the key.
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Gets the created date.(UTC).
	/// </summary>
	public DateTime CreatedDate { get; }

	/// <summary>
	/// Gets the timeout.
	/// </summary>
	public TimeSpan Timeout { get; }

	/// <summary>
	/// Gets a value indicating whether the entry has a sliding expiration.
	/// </summary>
	public bool IsSlidingExpiration { get; }

	/// <summary>
	/// Gets the last accessed date (UTC).
	/// </summary>
	public DateTime LastAccessedDate { get; private set; }

	/// <summary>
	/// Gets the hits.
	/// </summary>
	public int Hits { get; private set; }

	/// <summary>
	/// Adds a hit for the cache entry.
	/// </summary>
	/// <remarks>This method is not thread-safe and so the <see cref="Hits"/> may not be 100% accurate. We cannot lock here though as it could become a bottleneck.</remarks>
	public void AddHit()
	{
		Hits++;
		LastAccessedDate = DateTime.UtcNow;
	}
}