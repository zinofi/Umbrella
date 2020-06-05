using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Umbrella.Utilities.Options;

namespace Umbrella.Utilities.Caching.Abstractions
{
	/// <summary>
	/// A hybrid cache that allows cache items to be stored in an <see cref="IMemoryCache"/> or a <see cref="IDistributedCache"/> implementation.
	/// The cache includes the option to allow internal errors that occur when adding or retrieving items to be masked so that transient errors with the cache, e.g. a Redis error where
	/// the service is being restarted, does not cause a hard application failure.
	/// </summary>
	public interface IHybridCache
    {
		T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
		T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
		Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, CancellationToken cancellationToken = default, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
		Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
		Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CancellationToken cancellationToken = default, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
		Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
        (bool itemFound, T cacheItem) TryGetValue<T>(string cacheKey, HybridCacheMode cacheMode = HybridCacheMode.Memory);
        Task<(bool itemFound, T cacheItem)> TryGetValueAsync<T>(string cacheKey, CancellationToken cancellationToken = default, HybridCacheMode cacheMode = HybridCacheMode.Memory);
        T Set<T>(string cacheKey, T value, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
        T Set<T>(string cacheKey, T value, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
        Task<T> SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);
        Task<T> SetAsync<T>(string cacheKey, T value, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null);

		/// <summary>
		/// Gets all memory cache meta entries.
		/// </summary>
		/// <returns>A collection of <see cref="HybridCacheMetaEntry"/> instances.</returns>
		IReadOnlyCollection<HybridCacheMetaEntry> GetAllMemoryCacheMetaEntries();

		/// <summary>
		/// Removes the item with the specified <paramref name="cacheKey"/> from the cache.
		/// </summary>
		/// <typeparam name="T">The type of the cached item.</typeparam>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="cacheMode">The cache mode.</param>
		/// <returns>A task which can be awaited to indicate completion.</returns>
		Task RemoveAsync<T>(string cacheKey, CancellationToken cancellationToken = default, HybridCacheMode cacheMode = HybridCacheMode.Memory);

		/// <summary>
		/// Clears the memory cache.
		/// </summary>
		void ClearMemoryCache();
    }
}