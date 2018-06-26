using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    /// <summary>
    /// A set of extension methods for <see cref="IDistributedCache"/> instances to bring things more into line
    /// with those available for the <see cref="IMemoryCache"/>.
    /// </summary>
    public static class IDistributedCacheExtensions
    {
        #region Private Static Members
        private static JsonSerializerSettings s_JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        #endregion

        #region Public Static Members        
        /// <summary>
        /// Gets or sets the <see cref="JsonSerializerSettings"/> to use for serialization and deserialization of types. The default settings use a value
        /// for <see cref="JsonSerializerSettings.TypeNameHandling"/> of <see cref="TypeNameHandling.All"/>.
        /// </summary>
        /// <value>
        /// The <see cref="JsonSerializerSettings"/> instance.
        /// </value>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        public static JsonSerializerSettings JsonSettings
        {
            get => s_JsonSerializerSettings;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));

                s_JsonSerializerSettings = value;
            }
        }
        #endregion

        #region Public Static Methods        
        /// <summary>
        /// Gets the string from the cache using the specified <paramref name="key"/> or creates the specified string value using the provided <paramref name="factory"/> delegate if
        /// it is not present in the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string that has either been retrieved or added to the cache.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="cache"/>, <paramref name="key"/>,<paramref name="factory"/> or <paramref name="options"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="key"/> is either an empty string or whitespace.</exception>
        public static string GetOrCreateString(this IDistributedCache cache, string key, Func<string> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(options, nameof(options));

            string result = cache.GetString(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = factory();

                if (!string.IsNullOrWhiteSpace(result))
                    cache.SetString(key, result, options);
            }

            return result;
        }

        /// <summary>
        /// Gets the string from the cache using the specified <paramref name="key"/> or creates the specified string value using the provided <paramref name="factory"/> delegate if
        /// it is not present in the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The string that has either been retrieved or added to the cache.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="cache"/>, <paramref name="key"/>,<paramref name="factory"/> or <paramref name="options"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="key"/> is either an empty string or whitespace.</exception>
        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<Task<string>> factory, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(options, nameof(options));

            string result = await cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = await factory();

                if (!string.IsNullOrWhiteSpace(result))
                    await cache.SetStringAsync(key, result, options, cancellationToken);
            }

            return result;
        }

        public static (bool itemFound, TItem cacheItem, Exception exception) TryGetValue<TItem>(this IDistributedCache cache, string key)
            => TryGetValue<TItem>(cache, key, false);

        public static Task<(bool itemFound, TItem cacheItem, Exception exception)> TryGetValueAsync<TItem>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
            => TryGetValueAsync<TItem>(cache, key, cancellationToken, false);

        public static TItem Get<TItem>(this IDistributedCache cache, string key)
            => TryGetValue<TItem>(cache, key, true).cacheItem;

        public static async Task<TItem> GetAsync<TItem>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
            => (await TryGetValueAsync<TItem>(cache, key, cancellationToken, true)).cacheItem;

        public static void Set(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNull(item, nameof(item));
            Guard.ArgumentNotNull(options, nameof(options));

            string json = JsonConvert.SerializeObject(item, s_JsonSerializerSettings);

            cache.SetString(key, json, options);
        }

        public static Task SetAsync(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNull(item, nameof(item));
            Guard.ArgumentNotNull(options, nameof(options));

            string json = JsonConvert.SerializeObject(item, s_JsonSerializerSettings);

            return cache.SetStringAsync(key, json, options, cancellationToken);
        }

        public static TItem GetOrCreate<TItem>(this IDistributedCache cache, string key, Func<TItem> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(options, nameof(options));

            var (itemFound, cacheItem, exception) = cache.TryGetValue<TItem>(key);

            if (itemFound)
                return cacheItem;

            if (exception != null)
                throw new Exception($"{nameof(GetOrCreate)} failed", exception);

            // If we get this far then we haven't found the cached item
            TItem createdItem = factory();

            if (createdItem != null)
                Set(cache, key, createdItem, options);

            return createdItem;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> factory, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(cache, nameof(cache));
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(options, nameof(options));

            var (itemFound, cacheItem, exception) = await cache.TryGetValueAsync<TItem>(key, cancellationToken);

            if (itemFound)
                return cacheItem;

            if (exception != null)
                throw new Exception($"{nameof(GetOrCreateAsync)} failed", exception);

            // If we get this far then we haven't found the cached item
            TItem createdItem = await factory();

            if (createdItem != null)
                await SetAsync(cache, key, createdItem, options, cancellationToken);

            return createdItem;
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
                    var item = JsonConvert.DeserializeObject<TItem>(result, s_JsonSerializerSettings);

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
                string result = await cache.GetStringAsync(key, cancellationToken);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var item = JsonConvert.DeserializeObject<TItem>(result, s_JsonSerializerSettings);

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
        #endregion
    }
}