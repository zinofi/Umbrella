using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Json;

namespace Umbrella.Utilities.Caching
{
    /// <summary>
    /// A set of extension methods for <see cref="IDistributedCache"/> instances to bring things more into line
    /// with those available for the <see cref="IMemoryCache"/>.
    /// </summary>
    public static class IDistributedCacheExtensions
    {
        #region Public Static Methods        
        /// <summary>
        /// Gets the string from the cache using the specified <paramref name="key"/> or creates the specified string value using the provided <paramref name="factory"/> delegate if
        /// it is not present in the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="optionsBuilder">The options builder.</param>
        /// <param name="throwOnCacheFailure">
        /// <para>Specifies whether or not to throw an exception if the operation to get or set the cache item in the underlying cache fails.</para>
        /// <para>Setting this as false means that the failure is handled silently allowing the new cache item to be built with any cache failures being masked.
        /// Any exceptions are then returned by the method as an <see cref="UmbrellaDistributedCacheException"/> which has an inner exception of type <see cref="AggregateException"/> to be handled manually by the caller.
        /// </para>
        /// </param>
        /// <returns>The string that has either been retrieved or added to the cache together with any exception information</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="cache"/>, <paramref name="key"/>,<paramref name="factory"/> or <paramref name="optionsBuilder"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="key"/> is either an empty string or whitespace.</exception>
        public static (string item, UmbrellaDistributedCacheException exception) GetOrCreateString(this IDistributedCache cache, string key, Func<string> factory, Func<DistributedCacheEntryOptions> optionsBuilder, bool throwOnCacheFailure = true)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

            try
            {
                string result = null;

                List<Exception> lstException = throwOnCacheFailure ? new List<Exception>() : null;

                try
                {
                    result = cache.GetString(key);
                }
                catch (Exception exc)
                {
                    if (throwOnCacheFailure)
                        throw;

                    lstException.Add(exc);
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    //A failure to create the item should always throw an exception
                    result = factory();

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(result))
                            cache.SetString(key, result, optionsBuilder());
                    }
                    catch (Exception exc)
                    {
                        if (throwOnCacheFailure)
                            throw;

                        lstException.Add(exc);
                    }
                }

                return (result, CreateUmbrellaException(lstException));
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(GetOrCreateString)} failed.", exc);
            }
        }

        /// <summary>
        /// Gets the string from the cache using the specified <paramref name="key"/> or creates the specified string value using the provided <paramref name="factory"/> delegate if
        /// it is not present in the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="optionsBuilder">The options builder.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="throwOnCacheFailure">
        /// <para>Specifies whether or not to throw an exception if the operation to get or set the cache item in the underlying cache fails.</para>
        /// <para>Setting this as false means that the failure is handled silently allowing the new cache item to be built with any cache failures being masked.
        /// Any exceptions are then returned by the method as an <see cref="UmbrellaDistributedCacheException"/> which has an inner exception of type <see cref="AggregateException"/> to be handled manually by the caller.
        /// </para>
        /// </param>
        /// <returns>The string that has either been retrieved or added to the cache together with any exception information</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="cache"/>, <paramref name="key"/>,<paramref name="factory"/> or <paramref name="optionsBuilder"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="key"/> is either an empty string or whitespace.</exception>
        public static async Task<(string item, UmbrellaDistributedCacheException exception)> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<Task<string>> factory, Func<DistributedCacheEntryOptions> optionsBuilder, CancellationToken cancellationToken = default, bool throwOnCacheFailure = true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

            try
            {
                string result = null;

                List<Exception> lstException = throwOnCacheFailure ? new List<Exception>() : null;

                try
                {
                    result = await cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exc)
                {
                    if (throwOnCacheFailure)
                        throw;

                    lstException.Add(exc);
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    //A failure to create the item should always throw an exception
                    result = await factory().ConfigureAwait(false);

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(result))
                            await cache.SetStringAsync(key, result, optionsBuilder(), cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception exc)
                    {
                        if (throwOnCacheFailure)
                            throw;

                        lstException.Add(exc);
                    }
                }

                return (result, CreateUmbrellaException(lstException));
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(GetOrCreateString)} failed.", exc);
            }
        }

        public static (bool itemFound, TItem cacheItem, UmbrellaDistributedCacheException exception) TryGetValue<TItem>(this IDistributedCache cache, string key)
        {
            var (itemFound, cacheItem, exception) = TryGetValue<TItem>(cache, key, false);

            return (itemFound, cacheItem, exception != null ? new UmbrellaDistributedCacheException($"{nameof(TryGetValue)} failed.", exception) : null);
        }

        public static async Task<(bool itemFound, TItem cacheItem, UmbrellaDistributedCacheException exception)> TryGetValueAsync<TItem>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            var (itemFound, cacheItem, exception) = await TryGetValueAsync<TItem>(cache, key, cancellationToken, false).ConfigureAwait(false);

            return (itemFound, cacheItem, exception != null ? new UmbrellaDistributedCacheException($"{nameof(TryGetValueAsync)} failed.", exception) : null);
        }

        public static TItem Get<TItem>(this IDistributedCache cache, string key)
        {
            try
            {
                var (itemFound, cacheItem, exception) = TryGetValue<TItem>(cache, key, true);

                return cacheItem;
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(Get)} failed.", exc);
            }
        }

        public static async Task<TItem> GetAsync<TItem>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var (itemFound, cacheItem, exception) = await TryGetValueAsync<TItem>(cache, key, cancellationToken, true).ConfigureAwait(false);

                return cacheItem;
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(GetAsync)} failed.", exc);
            }
        }

        public static void Set(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNull(item, nameof(item));
            Guard.ArgumentNotNull(options, nameof(options));

            try
            {
                string json = UmbrellaStatics.SerializeJson(item, false, TypeNameHandling.All);

                cache.SetString(key, json, options);
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(Set)} failed.", exc);
            }
        }

        public static async Task SetAsync(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNull(item, nameof(item));
            Guard.ArgumentNotNull(options, nameof(options));

            try
            {
                string json = UmbrellaStatics.SerializeJson(item, false, TypeNameHandling.All);

                await cache.SetStringAsync(key, json, options, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(SetAsync)} failed.", exc);
            }
        }

        public static (TItem cacheItem, UmbrellaDistributedCacheException exception) GetOrCreate<TItem>(this IDistributedCache cache, string key, Func<TItem> factory, Func<DistributedCacheEntryOptions> optionsBuilder, bool throwOnCacheFailure = true)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

            try
            {
                List<Exception> lstException = throwOnCacheFailure ? new List<Exception>() : null;

                var (itemFound, cacheItem, exception) = cache.TryGetValue<TItem>(key);

                if (throwOnCacheFailure && exception != null)
                    throw exception;

                if (itemFound)
                    return (cacheItem, exception);

                if (exception != null)
                    lstException.Add(exception);

                // If we get this far then we haven't found the cached item
                // Always allow this to throw an exception
                TItem createdItem = factory();

                try
                {
                    if (createdItem != null)
                        Set(cache, key, createdItem, optionsBuilder());
                }
                catch(Exception exc)
                {
                    if (throwOnCacheFailure && exception != null)
                        throw;

                    lstException.Add(exc);
                }

                return (createdItem, CreateUmbrellaException(lstException));
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(GetOrCreate)} failed.", exc);
            }
        }

        public static async Task<(TItem cacheItem, UmbrellaDistributedCacheException exception)> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<CancellationToken, Task<TItem>> factory, Func<DistributedCacheEntryOptions> optionsBuilder, CancellationToken cancellationToken = default, bool throwOnCacheFailure = true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

            try
            {
                List<Exception> lstException = throwOnCacheFailure ? new List<Exception>() : null;

                var (itemFound, cacheItem, exception) = await cache.TryGetValueAsync<TItem>(key, cancellationToken).ConfigureAwait(false);

                if (throwOnCacheFailure && exception != null)
                    throw exception;

                if (itemFound)
                    return (cacheItem, exception);

                if (exception != null)
                    lstException.Add(exception);

                // If we get this far then we haven't found the cached item
                // Always allow this to throw an exception
                TItem createdItem = await factory(cancellationToken).ConfigureAwait(false);

                try
                {
                    if (createdItem != null)
                        await SetAsync(cache, key, createdItem, optionsBuilder(), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exc)
                {
                    if (throwOnCacheFailure && exception != null)
                        throw;

                    lstException.Add(exc);
                }

                return (createdItem, CreateUmbrellaException(lstException));
            }
            catch (Exception exc)
            {
                throw new UmbrellaDistributedCacheException($"{nameof(GetOrCreateAsync)} failed.", exc);
            }
        }
        #endregion

        #region Private Static Methods
        private static (bool itemFound, TItem cacheItem, Exception exception) TryGetValue<TItem>(this IDistributedCache cache, string key, bool throwError)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            try
            {
                string result = cache.GetString(key);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var item = UmbrellaStatics.DeserializeJson<TItem>(result, TypeNameHandling.All);

                    return (true, item, null);
                }
            }
            catch (Exception exc)
            {
                if (throwError)
                    throw;

                return (false, default, exc);
            }

            return (false, default, null);
        }

        private static async Task<(bool itemFound, TItem cacheItem, Exception exception)> TryGetValueAsync<TItem>(this IDistributedCache cache, string key, CancellationToken cancellationToken, bool throwError)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            try
            {
                string result = await cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var item = UmbrellaStatics.DeserializeJson<TItem>(result, TypeNameHandling.All);

                    return (true, item, null);
                }
            }
            catch (Exception exc)
            {
                if (throwError)
                    throw;

                return (false, default, exc);
            }

            return (false, default, null);
        }

        private static UmbrellaDistributedCacheException CreateUmbrellaException(List<Exception> exceptions)
            => exceptions?.Count > 0
                ? new UmbrellaDistributedCacheException("One or more errors have occurred. Please see the inner exception for details.", new AggregateException(exceptions))
                : null;
        #endregion
    }
}