using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Threading;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.Options;

namespace Umbrella.Utilities.Caching.Abstractions;

/// <summary>
/// A hybrid cache that allows cache items to be stored in an <see cref="IMemoryCache"/> or a <see cref="IDistributedCache"/> implementation.
/// The cache includes the option to allow internal errors that occur when adding or retrieving items to be masked so that transient errors with the cache, e.g. a Redis error where
/// the service is being restarted, does not cause a hard application failure.
/// </summary>
public interface IHybridCache
{
	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="expirationTimeSpanBuilder">The expiration time span builder.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="slidingExpiration">if set to <see langword="true" /> specifies that the item should have a sliding expiration.</param>
	/// <param name="throwOnCacheFailure">
	/// if set to <see langword="true" /> specified that an exception will be thrown if there is a problem accessing the cache.
	/// If this is <see langword="false" />, these failures will be handled silently and the <paramref name="actionFunction"/> will be used to build the return value.
	/// </param>
	/// <param name="priority">The priority.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, Func<TimeSpan>? expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null);

	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="options">The caching options.</param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null);

	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="expirationTimeSpanBuilder">The expiration time span builder.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="slidingExpiration">if set to <see langword="true" /> specifies that the item should have a sliding expiration.</param>
	/// <param name="throwOnCacheFailure">
	/// if set to <see langword="true" /> specified that an exception will be thrown if there is a problem accessing the cache.
	/// If this is <see langword="false" />, these failures will be handled silently and the <paramref name="actionFunction"/> will be used to build the return value.
	/// </param>
	/// <param name="priority">The priority.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, Func<TimeSpan>? expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="options">The caching options.</param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="expirationTimeSpanBuilder">The expiration time span builder.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="slidingExpiration">if set to <see langword="true" /> specifies that the item should have a sliding expiration.</param>
	/// <param name="throwOnCacheFailure">
	/// if set to <see langword="true" /> specified that an exception will be thrown if there is a problem accessing the cache.
	/// If this is <see langword="false" />, these failures will be handled silently and the <paramref name="actionFunction"/> will be used to build the return value.
	/// </param>
	/// <param name="priority">The priority.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, Func<TimeSpan>? expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the or creates a cache item.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="actionFunction">The action function.</param>
	/// <param name="options">The caching options.</param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The item that has been retrieved or just added to the cache.</returns>
	Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Tries to get the item from the cache with the specified <paramref name="cacheKey"/>.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <returns>A tuple specifying whether the item was found in the cache together with the cached item if it exists.</returns>
	(bool itemFound, T? cacheItem) TryGetValue<T>(string cacheKey, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool? cacheEnabledOverride = null);

	/// <summary>
	/// Tries to get the item from the cache with the specified <paramref name="cacheKey"/>.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple specifying whether the item was found in the cache together with the cached item if it exists.</returns>
	Task<(bool itemFound, T? cacheItem)> TryGetValueAsync<T>(string cacheKey, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool? cacheEnabledOverride = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Adds the specified item to the cache.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="expirationTimeSpan">The expiration time span.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="slidingExpiration">if set to <see langword="true" /> specifies that the item should have a sliding expiration.</param>
	/// <param name="throwOnCacheFailure">
	/// if set to <see langword="true" /> specified that an exception will be thrown if there is a problem accessing the cache.
	/// If this is <see langword="false" />, these failures will be handled silently and the <paramref name="value"/> will be returned.
	/// </param>
	/// <param name="priority">The priority.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <returns>The item added to the cache.</returns>
	T SetValue<T>(string cacheKey, T value, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null);

	/// <summary>
	/// Adds the specified item to the cache.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="options">The options.</param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <returns>The item added to the cache.</returns>
	T SetValue<T>(string cacheKey, T value, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null);

	/// <summary>
	/// Adds the specified item to the cache.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="expirationTimeSpan">The expiration time span.</param>
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="slidingExpiration">if set to <see langword="true" /> specifies that the item should have a sliding expiration.</param>
	/// <param name="throwOnCacheFailure">
	/// if set to <see langword="true" /> specified that an exception will be thrown if there is a problem accessing the cache.
	/// If this is <see langword="false" />, these failures will be handled silently and the <paramref name="value"/> will be returned.
	/// </param>
	/// <param name="priority">The priority.</param>
	/// <param name="cacheEnabledOverride">
	/// The cache enabled override. This is used to override the global cache setting in order to disable the caching.
	/// If <see cref="HybridCacheOptions.CacheEnabled"/> is set to false however, this parameter will be ignored, i.e. setting it to <see langword="true" /> will have no effect.
	/// </param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The item added to the cache.</returns>
	Task<T> SetValueAsync<T>(string cacheKey, T value, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Adds the specified item to the cache.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="options">The options.</param>
	/// <param name="expirationTokensBuilder">The expiration tokens builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The item added to the cache.</returns>
	Task<T> SetValueAsync<T>(string cacheKey, T value, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>?>? expirationTokensBuilder = null, CancellationToken cancellationToken = default);

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
	/// <param name="cacheMode">The cache mode.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task which can be awaited to indicate completion.</returns>
	Task RemoveAsync<T>(string cacheKey, HybridCacheMode cacheMode = HybridCacheMode.Memory, CancellationToken cancellationToken = default);

	/// <summary>
	/// Clears the memory cache.
	/// </summary>
	void ClearMemoryCache();
}