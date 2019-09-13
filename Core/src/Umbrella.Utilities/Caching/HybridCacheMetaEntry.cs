using System;
using System.Threading;

namespace Umbrella.Utilities.Caching
{
	public class HybridCacheMetaEntry
	{
		private int _hits;

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
		public int Hits => _hits;

		public void AddHit()
		{
			// TODO: Could we save any time here by running this op on a different thread to avoid blocking the caller?
			Interlocked.Increment(ref _hits);
			LastAccessedDate = DateTime.UtcNow;
		}
	}
}