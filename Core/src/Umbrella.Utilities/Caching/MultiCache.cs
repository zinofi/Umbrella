using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;

namespace Umbrella.Utilities.Caching
{
    // TODO: We could implement something to allow cache keys to be tracked and add a method to allow a key to be removed
    // from the cache. Would solve Matt's problem with not having any visibility of cache keys.
    // For the memory cache we can register a post-eviction callback so we can keep the list of keys up-to-date. Can't do that with the
    // distributed cache though.
    // Need some mechanism to clear other memory cache entries when running on a web farm though. Would be Azure specific though and
    // involve making http requests out to some endpoint with the azure app instance ids. Hmm...
    /// <summary>
    /// A multi cache that allows cache items to be stored in an <see cref="IMemoryCache"/> or a <see cref="IDistributedCache"/> implementation.
    /// The cache includes the option to allow internal errors that occur when adding or retrieving items to be masked.
    /// </summary>
    /// <seealso cref="IMultiCache" />
    public class MultiCache : IMultiCache
    {
        protected ILogger Log { get; }
        protected MultiCacheOptions Options { get; }
        protected bool TrackKeys { get; }
        protected bool TrackKeysAndHits { get; }
        protected IDistributedCache DistributedCache { get; }
        protected IMemoryCache MemoryCache { get; }
        protected ConcurrentDictionary<string, MultiCacheMetaEntry> MemoryCacheMetaEntryDictionary { get; } = new ConcurrentDictionary<string, MultiCacheMetaEntry>();

        public MultiCache(ILogger<MultiCache> logger,
            MultiCacheOptions options,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache)
        {
            Log = logger;
            Options = options;
            DistributedCache = distributedCache;
            MemoryCache = memoryCache;

            TrackKeys = (Options.AnalyticsMode & MultiCacheAnalyticsMode.TrackKeys) == MultiCacheAnalyticsMode.TrackKeys;
            TrackKeysAndHits = (Options.AnalyticsMode & MultiCacheAnalyticsMode.TrackKeysAndHits) == MultiCacheAnalyticsMode.TrackKeysAndHits;
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, Func<TimeSpan> expirationTimeSpanBuilder = null, bool useMemoryCache = true, bool slidingExpiration = false, bool throwOnCacheFailure = false, CacheItemPriority priority = CacheItemPriority.Normal, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null, bool? cacheEnabledOverride = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            Guard.ArgumentNotNull(actionFunction, nameof(actionFunction));

            try
            {
                bool cacheEnabled = Options.CacheEnabled;

                if (cacheEnabledOverride.HasValue)
                    cacheEnabled = cacheEnabledOverride.Value;

                if (cacheEnabled)
                {
                    string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                    if (!useMemoryCache)
                    {
                        try
                        {
                            var (cacheItem, exception) = DistributedCache.GetOrCreate(cacheKeyInternal, actionFunction, () => BuildDistributedCacheEntryOptions(expirationTimeSpanBuilder, slidingExpiration), false);

                            if (exception != null)
                            {
                                if (!throwOnCacheFailure)
                                {
                                    Log.WriteError(exception, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority });
                                }
                                else
                                {
                                    throw exception;
                                }
                            }

                            return cacheItem;
                        }
                        catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: !throwOnCacheFailure))
                        {
                            // Silently mask the exception when not throwing on cache failure.
                        }
                    }
                    else
                    {
                        try
                        {
                            MultiCacheMetaEntry cacheMetaEntry = null;

                            if (TrackKeys)
                                MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out cacheMetaEntry);

                            T cacheItem = MemoryCache.GetOrCreate(cacheKeyInternal, entry =>
                            {
                                TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.MaxCacheTimeout;

                                MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);
                                entry.SetOptions(options);

                                T entryToAdd = actionFunction();

                                if (entryToAdd != null && TrackKeys)
                                {
                                    cacheMetaEntry = new MultiCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration);
                                    MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, cacheMetaEntry);
                                    entry.RegisterPostEvictionCallback((key, value, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out MultiCacheMetaEntry removedEntry));
                                }

                                return entryToAdd;
                            });

                            if (cacheMetaEntry != null && TrackKeysAndHits)
                                cacheMetaEntry.AddHit();

                            return cacheItem;
                        }
                        catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: !throwOnCacheFailure))
                        {
                            // Silently mask the exception when not throwing on cache failure.
                        }
                    }
                }

                return actionFunction();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: true))
            {
                // If we get this far then there has definitely been a problem with the actionFunction. We need to always throw here.
                throw new MultiCacheException("There has been a problem with the cache.", exc);
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CancellationToken cancellationToken = default, Func<TimeSpan> expirationTimeSpanBuilder = null, bool useMemoryCache = true, bool slidingExpiration = false, bool throwOnCacheFailure = false, CacheItemPriority priority = CacheItemPriority.Normal, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null, bool? cacheEnabledOverride = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            Guard.ArgumentNotNull(actionFunction, nameof(actionFunction));

            try
            {
                bool cacheEnabled = Options.CacheEnabled;

                if (cacheEnabledOverride.HasValue)
                    cacheEnabled = cacheEnabledOverride.Value;

                if (cacheEnabled)
                {
                    string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                    if (!useMemoryCache)
                    {
                        try
                        {
                            var (cacheItem, exception) = await DistributedCache.GetOrCreateAsync(cacheKeyInternal, actionFunction, () => BuildDistributedCacheEntryOptions(expirationTimeSpanBuilder, slidingExpiration), cancellationToken, false).ConfigureAwait(false);

                            if (!throwOnCacheFailure)
                            {
                                Log.WriteError(exception, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority });
                            }
                            else
                            {
                                throw exception;
                            }

                            return cacheItem;
                        }
                        catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: !throwOnCacheFailure))
                        {
                            // Silently mask the exception when not throwing on cache failure.
                        }
                    }
                    else
                    {
                        try
                        {
                            MultiCacheMetaEntry cacheMetaEntry = null;

                            if (TrackKeys)
                                MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out cacheMetaEntry);

                            T cacheItem = await MemoryCache.GetOrCreateAsync(cacheKeyInternal, async entry =>
                            {
                                TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.MaxCacheTimeout;

                                MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);
                                entry.SetOptions(options);

                                T entryToAdd = await actionFunction().ConfigureAwait(false);

                                if (entryToAdd != null && TrackKeys)
                                {
                                    cacheMetaEntry = new MultiCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration);
                                    MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, cacheMetaEntry);
                                    entry.RegisterPostEvictionCallback((key, value, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out MultiCacheMetaEntry removedEntry));
                                }

                                return entryToAdd;

                            }).ConfigureAwait(false);

                            if (cacheMetaEntry != null && TrackKeysAndHits)
                                cacheMetaEntry.AddHit();

                            return cacheItem;
                        }
                        catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: !throwOnCacheFailure))
                        {
                            // Silently mask the exception when not throwing on cache failure.
                        }
                    }
                }

                return await actionFunction();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: true))
            {
                // If we get this far then there has definitely been a problem with the actionFunction. We need to throw here.
                throw new MultiCacheException("There has been a problem with the cache.", exc);
            }
        }

        public (bool itemFound, T cacheItem) TryGetValue<T>(string cacheKey, bool useMemoryCache = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            try
            {
                string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                if (!useMemoryCache)
                {
                    var (itemFound, cacheItem, exception) = DistributedCache.TryGetValue<T>(cacheKeyInternal);

                    if (exception != null)
                        Log.WriteError(exception, new { cacheKey, useMemoryCache });

                    return (itemFound, cacheItem);
                }
                else
                {
                    bool found = MemoryCache.TryGetValue(cacheKeyInternal, out T value);

                    if (found && TrackKeysAndHits && MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out MultiCacheMetaEntry cacheMetaEntry))
                        cacheMetaEntry.AddHit();

                    return (found, value);
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, useMemoryCache }, returnValue: true))
            {
                return default;
            }
        }

        public async Task<(bool itemFound, T cacheItem)> TryGetValueAsync<T>(string cacheKey, CancellationToken cancellationToken = default, bool useMemoryCache = true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            try
            {
                string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                if (!useMemoryCache)
                {
                    var (itemFound, cacheItem, exception) = await DistributedCache.TryGetValueAsync<T>(cacheKeyInternal, cancellationToken).ConfigureAwait(false);

                    if (exception != null)
                        Log.WriteError(exception, new { cacheKey, cacheKeyInternal, useMemoryCache });

                    return (itemFound, cacheItem);
                }
                else
                {
                    bool found = MemoryCache.TryGetValue(cacheKeyInternal, out T value);

                    if (found && TrackKeysAndHits && MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out MultiCacheMetaEntry cacheMetaEntry))
                        cacheMetaEntry.AddHit();

                    return (found, value);
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, useMemoryCache }, returnValue: true))
            {
                return default;
            }
        }

        public T Set<T>(string cacheKey, T value, TimeSpan expirationTimeSpan, bool useMemoryCache = true, bool slidingExpiration = false, bool throwOnCacheFailure = false, CacheItemPriority priority = CacheItemPriority.Normal, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            Guard.ArgumentNotNull(value, nameof(value));

            try
            {
                string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                if (!useMemoryCache)
                {
                    var options = BuildDistributedCacheEntryOptions(expirationTimeSpan, slidingExpiration);

                    DistributedCache.Set(cacheKeyInternal, value, options);
                }
                else
                {
                    var options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);

                    if(TrackKeys)
                        options.RegisterPostEvictionCallback((key, cachedValue, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out MultiCacheMetaEntry removedEntry));

                    MemoryCache.Set(cacheKeyInternal, value, options);

                    if (TrackKeys)
                        MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, new MultiCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration));
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, expirationTimeSpan, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: true))
            {
                if (throwOnCacheFailure)
                    throw new MultiCacheException("There has been a problem setting the specified item in the cache.", exc);
            }

            return value;
        }

        public async Task<T> SetAsync<T>(string cacheKey, T value, TimeSpan expirationTimeSpan, CancellationToken cancellationToken = default, bool useMemoryCache = true, bool slidingExpiration = false, bool throwOnCacheFailure = false, CacheItemPriority priority = CacheItemPriority.Normal, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            try
            {
                string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

                if (!useMemoryCache)
                {
                    var options = BuildDistributedCacheEntryOptions(expirationTimeSpan, slidingExpiration);

                    await DistributedCache.SetAsync(cacheKeyInternal, value, options, cancellationToken);
                }
                else
                {
                    var options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);

                    if (TrackKeys)
                        options.RegisterPostEvictionCallback((key, cachedValue, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out MultiCacheMetaEntry removedEntry));

                    MemoryCache.Set(cacheKeyInternal, value, options);

                    if (TrackKeys)
                        MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, new MultiCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration));
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, expirationTimeSpan, useMemoryCache, slidingExpiration, throwOnCacheFailure, priority }, returnValue: true))
            {
                if (throwOnCacheFailure)
                    throw new MultiCacheException("There has been a problem setting the specified item in the cache.", exc);
            }

            return value;
        }

        public IReadOnlyCollection<MultiCacheMetaEntry> GetAllMemoryCacheMetaEntries()
        {
            if (!TrackKeys)
                throw new MultiCacheException("Meta entries cannot be retrieved when analytics is disabled.");

            try
            {
                return MemoryCacheMetaEntryDictionary.Select(x => x.Value).ToList();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw new MultiCacheException("There has been a problem reading the memory cache keys.", exc);
            }
        }

        protected virtual string CreateCacheKey<T>(string key)
            => Options.CacheKeyBuilder?.Invoke(typeof(T), key)?.ToUpperInvariant() ?? key.ToUpperInvariant();

        private DistributedCacheEntryOptions BuildDistributedCacheEntryOptions(Func<TimeSpan> expirationTimeSpanBuilder, bool slidingExpiration)
        {
            TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.MaxCacheTimeout;

            return BuildDistributedCacheEntryOptions(expirationTimeSpan, slidingExpiration);
        }

        private DistributedCacheEntryOptions BuildDistributedCacheEntryOptions(in TimeSpan expirationTimeSpan, bool slidingExpiration)
        {
            var options = new DistributedCacheEntryOptions();

            if (slidingExpiration)
            {
                options.SetSlidingExpiration(expirationTimeSpan);
            }
            else
            {
                options.SetAbsoluteExpiration(expirationTimeSpan);
            }

            return options;
        }

        //TODO: Why was this added??
        //private MemoryCacheEntryOptions BuildMemoryCacheEntryOptions(TimeSpan expirationTimeSpan, bool slidingExpiration, CacheItemPriority priority, Func<IEnumerable<IChangeToken>> expirationTokensBuilder)
        //    => BuildMemoryCacheEntryOptions(in expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);

        private MemoryCacheEntryOptions BuildMemoryCacheEntryOptions(in TimeSpan expirationTimeSpan, bool slidingExpiration, CacheItemPriority priority, Func<IEnumerable<IChangeToken>> expirationTokensBuilder)
        {
            var options = new MemoryCacheEntryOptions();

            if (slidingExpiration)
            {
                options.SetSlidingExpiration(expirationTimeSpan);
            }
            else
            {
                options.SetAbsoluteExpiration(expirationTimeSpan);
            }

            options.SetPriority(priority);

            if (expirationTokensBuilder != null)
            {
                IEnumerable<IChangeToken> tokens = expirationTokensBuilder();

                if (tokens != null)
                {
                    foreach (var token in tokens)
                    {
                        if (token != null)
                            options.AddExpirationToken(token);
                    }
                }
            }

            return options;
        }
    }
}