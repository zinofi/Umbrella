﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Umbrella.Utilities.Abstractions;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;

namespace Umbrella.Utilities.Caching
{
	/// <summary>
	/// A multi cache that allows cache items to be stored in an <see cref="IMemoryCache"/> or a <see cref="IDistributedCache"/> implementation.
	/// The cache includes the option to allow internal errors that occur when adding or retrieving items from the caches to be masked in the event of transient errors.
	/// </summary>
	/// <seealso cref="IHybridCache" />
	public class HybridCache : IHybridCache, IDisposable
	{
		private readonly ReaderWriterLockSlim _nukeReaderWriterLock = new ReaderWriterLockSlim();
		private CancellationTokenSource _nukeTokenSource = new CancellationTokenSource();

		protected ILogger Log { get; }
		protected HybridCacheOptions Options { get; }
		protected bool TrackKeys { get; }
		protected bool TrackKeysAndHits { get; }
		protected IDistributedCache DistributedCache { get; }
		protected IMemoryCache MemoryCache { get; }
		protected ConcurrentDictionary<string, HybridCacheMetaEntry> MemoryCacheMetaEntryDictionary { get; } = new ConcurrentDictionary<string, HybridCacheMetaEntry>();

		public HybridCache(ILogger<HybridCache> logger,
			HybridCacheOptions options,
			IDistributedCache distributedCache,
			IMemoryCache memoryCache)
		{
			Log = logger;
			Options = options;
			DistributedCache = distributedCache;
			MemoryCache = memoryCache;

			TrackKeys = (Options.AnalyticsMode & HybridCacheAnalyticsMode.TrackKeys) == HybridCacheAnalyticsMode.TrackKeys;
			TrackKeysAndHits = (Options.AnalyticsMode & HybridCacheAnalyticsMode.TrackKeysAndHits) == HybridCacheAnalyticsMode.TrackKeysAndHits;
		}

		public T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
		{
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
			Guard.ArgumentNotNull(actionFunction, nameof(actionFunction));

			try
			{
				if (IsCacheEnabled(cacheEnabledOverride))
				{
					string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

					if (cacheMode == HybridCacheMode.Distributed)
					{
						try
						{
							(T cacheItem, UmbrellaDistributedCacheException exception) = DistributedCache.GetOrCreate(cacheKeyInternal, actionFunction, () => BuildDistributedCacheEntryOptions(expirationTimeSpanBuilder, slidingExpiration), false);

							if (exception != null)
							{
								if (!throwOnCacheFailure)
								{
									Log.WriteError(exception, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride });
								}
								else
								{
									throw exception;
								}
							}

							return cacheItem;
						}
						catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
						{
							if (throwOnCacheFailure)
								throw new HybridCacheException("There has been a problem getting or creating the specified item in the cache.", exc);
						}
					}
					else
					{
						try
						{
							HybridCacheMetaEntry cacheMetaEntry = null;

							if (TrackKeys)
								MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out cacheMetaEntry);

							T cacheItem = MemoryCache.GetOrCreate(cacheKeyInternal, entry =>
							{
								TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.DefaultCacheTimeout;

								MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);
								entry.SetOptions(options);

								T entryToAdd = actionFunction();

								if (entryToAdd != null && TrackKeys)
								{
									cacheMetaEntry = new HybridCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration);
									MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, cacheMetaEntry);
									entry.RegisterPostEvictionCallback((key, value, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out HybridCacheMetaEntry removedEntry));
								}

								return entryToAdd;
							});

							if (cacheMetaEntry != null && TrackKeysAndHits)
								cacheMetaEntry.AddHit();

							return cacheItem;
						}
						catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
						{
							if (throwOnCacheFailure)
								throw new HybridCacheException("There has been a problem getting or creating the specified item in the cache.", exc);
						}
					}
				}

				return actionFunction();
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
			{
				// If we get this far then there has definitely been a problem with the actionFunction. We need to always throw here.
				throw new HybridCacheException("There has been a problem with the cache.", exc);
			}
		}

		public T GetOrCreate<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
			=> GetOrCreate(cacheKey, actionFunction, () => options.CacheTimeout, options.CacheMode, options.CacheSlidingExpiration, options.CacheThrowOnFailure, options.CachePriority, options.CacheEnabled, expirationTokensBuilder);

		public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, CancellationToken cancellationToken = default, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
			Guard.ArgumentNotNull(actionFunction, nameof(actionFunction));

			try
			{
				return await GetOrCreateAsync(cacheKey, () => Task.FromResult(actionFunction()), cancellationToken, expirationTimeSpanBuilder, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride, expirationTokensBuilder);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
			{
				throw new HybridCacheException("There has been a problem with the cache.", exc);
			}
		}

		public Task<T> GetOrCreateAsync<T>(string cacheKey, Func<T> actionFunction, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
			=> GetOrCreateAsync(cacheKey, actionFunction, cancellationToken, () => options.CacheTimeout, options.CacheMode, options.CacheThrowOnFailure, options.CacheThrowOnFailure, options.CachePriority, options.CacheEnabled,expirationTokensBuilder);

		public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CancellationToken cancellationToken = default, Func<TimeSpan> expirationTimeSpanBuilder = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
			Guard.ArgumentNotNull(actionFunction, nameof(actionFunction));

			try
			{
				if (IsCacheEnabled(cacheEnabledOverride))
				{
					string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

					if (cacheMode == HybridCacheMode.Distributed)
					{
						try
						{
							(T cacheItem, UmbrellaDistributedCacheException exception) = await DistributedCache.GetOrCreateAsync(cacheKeyInternal, actionFunction, () => BuildDistributedCacheEntryOptions(expirationTimeSpanBuilder, slidingExpiration), cancellationToken, false).ConfigureAwait(false);

							if (!throwOnCacheFailure)
							{
								Log.WriteError(exception, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride });
							}
							else
							{
								throw exception;
							}

							return cacheItem;
						}
						catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
						{
							if (throwOnCacheFailure)
								throw new HybridCacheException("There has been a problem getting or creating the specified item in the cache.", exc);
						}
					}
					else
					{
						try
						{
							HybridCacheMetaEntry cacheMetaEntry = null;

							if (TrackKeys)
								MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out cacheMetaEntry);
							
							// TODO: Investigate using AsyncLazy<T> to ensure that the factory only executes once. Internally, MemoryCache doesn't use locking
							// so the factory could run multiple times. Only a problem if the factory is expensive though.
							// Replace the boolean 'useMemoryCache' with an enum: CacheMode: Memory, MemoryMutex, Distributed, DistributedMutex
							T cacheItem = await MemoryCache.GetOrCreateAsync(cacheKeyInternal, async entry =>
							{
								TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.DefaultCacheTimeout;

								MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(expirationTimeSpan, slidingExpiration, priority, expirationTokensBuilder);
								entry.SetOptions(options);

								T entryToAdd = await actionFunction().ConfigureAwait(false);

								if (entryToAdd != null && TrackKeys)
								{
									cacheMetaEntry = new HybridCacheMetaEntry(cacheKeyInternal, expirationTimeSpan, slidingExpiration);
									MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, cacheMetaEntry);
									entry.RegisterPostEvictionCallback((key, value, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out HybridCacheMetaEntry removedEntry));
								}

								return entryToAdd;

							}).ConfigureAwait(false);

							if (cacheMetaEntry != null && TrackKeysAndHits)
								cacheMetaEntry.AddHit();

							return cacheItem;
						}
						catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheKeyInternal, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
						{
							if(throwOnCacheFailure)
								throw new HybridCacheException("There has been a problem getting or creating the specified item in the cache.", exc);
						}
					}
				}

				return await actionFunction();
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
			{
				// If we get this far then there has definitely been a problem with the actionFunction. We need to throw here.
				throw new HybridCacheException("There has been a problem with the cache.", exc);
			}
		}

		public Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> actionFunction, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
			=> GetOrCreateAsync(cacheKey, actionFunction, cancellationToken, () => options.CacheTimeout, options.CacheMode, options.CacheSlidingExpiration, options.CacheThrowOnFailure, options.CachePriority, options.CacheEnabled, expirationTokensBuilder);

		public (bool itemFound, T cacheItem) TryGetValue<T>(string cacheKey, HybridCacheMode cacheMode = HybridCacheMode.Memory)
		{
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

			try
			{
				string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

				if (cacheMode == HybridCacheMode.Distributed)
				{
					(bool itemFound, T cacheItem, UmbrellaDistributedCacheException exception) = DistributedCache.TryGetValue<T>(cacheKeyInternal);

					if (exception != null)
						Log.WriteError(exception, new { cacheKey, cacheMode });

					return (itemFound, cacheItem);
				}
				else
				{
					bool found = MemoryCache.TryGetValue(cacheKeyInternal, out T value);

					if (found && TrackKeysAndHits && MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out HybridCacheMetaEntry cacheMetaEntry))
						cacheMetaEntry.AddHit();

					return (found, value);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheMode }, returnValue: true))
			{
				return default;
			}
		}

		public async Task<(bool itemFound, T cacheItem)> TryGetValueAsync<T>(string cacheKey, CancellationToken cancellationToken = default, HybridCacheMode cacheMode = HybridCacheMode.Memory)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

			try
			{
				string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

				if (cacheMode == HybridCacheMode.Distributed)
				{
					(bool itemFound, T cacheItem, UmbrellaDistributedCacheException exception) = await DistributedCache.TryGetValueAsync<T>(cacheKeyInternal, cancellationToken).ConfigureAwait(false);

					if (exception != null)
						Log.WriteError(exception, new { cacheKey, cacheKeyInternal, cacheMode });

					return (itemFound, cacheItem);
				}
				else
				{
					bool found = MemoryCache.TryGetValue(cacheKeyInternal, out T value);

					if (found && TrackKeysAndHits && MemoryCacheMetaEntryDictionary.TryGetValue(cacheKeyInternal, out HybridCacheMetaEntry cacheMetaEntry))
						cacheMetaEntry.AddHit();

					return (found, value);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, cacheMode }, returnValue: true))
			{
				return default;
			}
		}

		public T Set<T>(string cacheKey, T value, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
		{
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
			Guard.ArgumentNotNull(value, nameof(value));

			try
			{
				if (!IsCacheEnabled(cacheEnabledOverride))
					return value;

				string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

				TimeSpan tsExpiration = expirationTimeSpan ?? Options.DefaultCacheTimeout;

				if (cacheMode == HybridCacheMode.Distributed)
				{
					DistributedCacheEntryOptions options = BuildDistributedCacheEntryOptions(tsExpiration, slidingExpiration);

					DistributedCache.Set(cacheKeyInternal, value, options);
				}
				else
				{
					MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(tsExpiration, slidingExpiration, priority, expirationTokensBuilder);

					if (TrackKeys)
						options.RegisterPostEvictionCallback((key, cachedValue, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out HybridCacheMetaEntry removedEntry));

					MemoryCache.Set(cacheKeyInternal, value, options);

					if (TrackKeys)
						MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, new HybridCacheMetaEntry(cacheKeyInternal, tsExpiration, slidingExpiration));
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, expirationTimeSpan, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
			{
				if (throwOnCacheFailure)
					throw new HybridCacheException("There has been a problem setting the specified item in the cache.", exc);
			}

			return value;
		}

		public T Set<T>(string cacheKey, T value, CacheableUmbrellaOptions options, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
			=> Set(cacheKey, value, options.CacheTimeout, options.CacheMode, options.CacheSlidingExpiration, options.CacheThrowOnFailure, options.CachePriority, options.CacheEnabled, expirationTokensBuilder);

		public async Task<T> SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default, TimeSpan? expirationTimeSpan = null, HybridCacheMode cacheMode = HybridCacheMode.Memory, bool slidingExpiration = false, bool throwOnCacheFailure = true, CacheItemPriority priority = CacheItemPriority.Normal, bool? cacheEnabledOverride = null, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

			try
			{
				if (!IsCacheEnabled(cacheEnabledOverride))
					return value;

				string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

				TimeSpan tsExpiration = expirationTimeSpan ?? Options.DefaultCacheTimeout;

				if (cacheMode == HybridCacheMode.Distributed)
				{
					DistributedCacheEntryOptions options = BuildDistributedCacheEntryOptions(tsExpiration, slidingExpiration);

					await DistributedCache.SetAsync(cacheKeyInternal, value, options, cancellationToken);
				}
				else
				{
					MemoryCacheEntryOptions options = BuildMemoryCacheEntryOptions(tsExpiration, slidingExpiration, priority, expirationTokensBuilder);

					if (TrackKeys)
						options.RegisterPostEvictionCallback((key, cachedValue, reason, state) => MemoryCacheMetaEntryDictionary.TryRemove(cacheKeyInternal, out HybridCacheMetaEntry removedEntry));

					MemoryCache.Set(cacheKeyInternal, value, options);

					if (TrackKeys)
						MemoryCacheMetaEntryDictionary.TryAdd(cacheKeyInternal, new HybridCacheMetaEntry(cacheKeyInternal, tsExpiration, slidingExpiration));
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey, expirationTimeSpan, cacheMode, slidingExpiration, throwOnCacheFailure, priority, cacheEnabledOverride }, returnValue: true))
			{
				if (throwOnCacheFailure)
					throw new HybridCacheException("There has been a problem setting the specified item in the cache.", exc);
			}

			return value;
		}

		public Task<T> SetAsync<T>(string cacheKey, T value, CacheableUmbrellaOptions options, CancellationToken cancellationToken = default, Func<IEnumerable<IChangeToken>> expirationTokensBuilder = null)
			=> SetAsync(cacheKey, value, cancellationToken, options.CacheTimeout, options.CacheMode, options.CacheSlidingExpiration, options.CacheThrowOnFailure, options.CachePriority, options.CacheEnabled, expirationTokensBuilder);

		/// <summary>
		/// Gets all memory cache meta entries.
		/// </summary>
		/// <returns>A collection of <see cref="HybridCacheMetaEntry"/> instances.</returns>
		/// <exception cref="HybridCacheException">
		/// Meta entries cannot be retrieved when analytics is disabled.
		/// or
		/// There has been a problem reading the memory cache keys.
		/// </exception>
		public IReadOnlyCollection<HybridCacheMetaEntry> GetAllMemoryCacheMetaEntries()
		{
			if (!TrackKeys)
				throw new HybridCacheException("Meta entries cannot be retrieved when analytics is disabled.");

			try
			{
				// TODO: Find a way of getting information about entries in the DistributedCache.
				// Will need to create an abstraction to allow for this and have specific implementations, e.g. Redis
				// Also, create a Remove and RemoveAll method (and async variants) to allow individual keys to be removed.
				// Need some way of accessing the memory caches on a server farm too.
				return MemoryCacheMetaEntryDictionary.Select(x => x.Value).ToList();
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new HybridCacheException("There has been a problem reading the memory cache keys.", exc);
			}
		}

		/// <summary>
		/// Removes the item with the specified <paramref name="cacheKey"/> from the cache.
		/// </summary>
		/// <typeparam name="T">The type of the cached item.</typeparam>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task which can be awaited to indicate completion.</returns>
		/// <exception cref="HybridCacheException">There has been a problem removing the item with the key: " + <paramref name="cacheKey"/></exception>
		public async Task RemoveAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

			try
			{
				string cacheKeyInternal = CreateCacheKey<T>(cacheKey);

				MemoryCache.Remove(cacheKeyInternal);
				await DistributedCache.RemoveAsync(cacheKeyInternal, cancellationToken);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheKey }, returnValue: true))
			{
				throw new HybridCacheException("There has been a problem removing the item with the key: " + cacheKey, exc);
			}
		}

		/// <summary>
		/// Clears the memory cache.
		/// </summary>
		/// <exception cref="HybridCacheException">There was a problem clearing all items from the memory cache.</exception>
		public void ClearMemoryCache()
		{
			_nukeReaderWriterLock.EnterWriteLock();

			try
			{
				_nukeTokenSource.Cancel();
				_nukeTokenSource.Dispose();

				// Reset things so that all future items added to the MemoryCache use a new CancellationToken.
				_nukeTokenSource = new CancellationTokenSource();
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new HybridCacheException("There was a problem clearing all items from the memory cache.", exc);
			}
			finally
			{
				_nukeReaderWriterLock.ExitWriteLock();
			}
		}

		protected virtual string CreateCacheKey<T>(string key)
			=> Options.CacheKeyBuilder?.Invoke(typeof(T), key)?.ToUpperInvariant() ?? key.ToUpperInvariant();

		private bool IsCacheEnabled(bool? cacheEnabledOverride)
			=> Options.CacheEnabled && cacheEnabledOverride.HasValue ? cacheEnabledOverride.Value : Options.CacheEnabled;

		private DistributedCacheEntryOptions BuildDistributedCacheEntryOptions(Func<TimeSpan> expirationTimeSpanBuilder, bool slidingExpiration)
		{
			TimeSpan expirationTimeSpan = expirationTimeSpanBuilder?.Invoke() ?? Options.DefaultCacheTimeout;

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
					foreach (IChangeToken token in tokens)
					{
						if (token != null)
							options.AddExpirationToken(token);
					}
				}
			}

			_nukeReaderWriterLock.EnterReadLock();

			try
			{
				options.AddExpirationToken(new CancellationChangeToken(_nukeTokenSource.Token));
			}
			finally
			{
				_nukeReaderWriterLock.ExitReadLock();
			}

			return options;
		}

		#region IDisposable Support
		private bool _isDisposed = false; // To detect redundant calls

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_nukeTokenSource.Cancel();
					_nukeTokenSource.Dispose();
					_nukeReaderWriterLock.Dispose();
				}

				_isDisposed = true;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			try
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(true);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new HybridCacheException("There has been a problem disposing this instance.", exc);
			}
		}
		#endregion
	}
}