using System;

namespace Umbrella.Utilities.Caching
{
	/// <summary>
	/// Represents the meta-data of an entry stored in the <see cref="HybridCache"/> when the cached item is stored in-memory.
	/// No such entries are created when items are stored in a distributed cache as they cannot be reliably tracked.
	/// </summary>
	public class HybridCacheMetaEntry
	{
		public HybridCacheMetaEntry(string key, in TimeSpan timeout, bool isSlidingExpiration)
		{
			Key = key;
			Timeout = timeout;
			IsSlidingExpiration = isSlidingExpiration;
			CreatedDate = DateTime.UtcNow;
		}

		public string Key { get; }
		public DateTime CreatedDate { get; }
		public TimeSpan Timeout { get; }
		public bool IsSlidingExpiration { get; }
		public DateTime LastAccessedDate { get; private set; }
		public int Hits { get; private set; }

		public void AddHit()
		{
			// NB: Multiple threads could cause these values to not be 100% accurate. Can't lock here though as it
			// could become a bottleneck.
			Hits++;
			LastAccessedDate = DateTime.UtcNow;
		}
	}
}