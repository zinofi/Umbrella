using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    public static class IDistributedCacheExtensions
    {
        #region Private Static Members
        private static readonly DistributedCacheEntryOptions s_DefaultOptions = new DistributedCacheEntryOptions();
        #endregion

        #region Public Static Methods
        public static string GetOrSetString(this IDistributedCache cache, string key, Func<string> factory, DistributedCacheEntryOptions options = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = cache.GetString(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = factory();

                if (!string.IsNullOrWhiteSpace(result))
                    cache.SetString(key, result, options ?? s_DefaultOptions);
            }

            return result;
        }

        public static async Task<string> GetOrSetStringAsync(this IDistributedCache cache, string key, Func<Task<string>> factory, DistributedCacheEntryOptions options = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = await factory();

                if (!string.IsNullOrWhiteSpace(result))
                    await cache.SetStringAsync(key, result, options ?? s_DefaultOptions);
            }

            return result;
        }

        public static (bool itemFound, TItem cacheItem) TryGetFromJsonString<TItem>(this IDistributedCache cache, string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = cache.GetString(key);

            if (!string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    var item = JsonConvert.DeserializeObject<TItem>(result);

                    return (true, item);
                }
                catch (Exception)
                {
                    cache.Remove(key);
                    throw;
                }
            }

            return (false, default);
        }

        public static async Task<(bool itemFound, TItem cacheItem)> TryGetFromJsonStringAsync<TItem>(this IDistributedCache cache, string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);

            if (!string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    var item = JsonConvert.DeserializeObject<TItem>(result);

                    return (true, item);
                }
                catch (Exception)
                {
                    await cache.RemoveAsync(key);
                    throw;
                }
            }

            return (false, default);
        }

        public static TItem GetFromJsonString<TItem>(this IDistributedCache cache, string key) => cache.TryGetFromJsonString<TItem>(key).cacheItem;
        public static async Task<TItem> GetFromJsonStringAsync<TItem>(this IDistributedCache cache, string key) => (await cache.TryGetFromJsonStringAsync<TItem>(key)).cacheItem;

        public static void SetAsJsonString(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options = null, JsonSerializerSettings settings = null)
        {
            Guard.ArgumentNotNull(item, nameof(item));

            string json = settings == null
                ? JsonConvert.SerializeObject(item)
                : JsonConvert.SerializeObject(item, settings);

            cache.SetString(key, json, options ?? s_DefaultOptions);
        }

        public static Task SetAsJsonStringAsync(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options = null, JsonSerializerSettings settings = null)
        {
            Guard.ArgumentNotNull(item, nameof(item));

            string json = settings == null
                ? JsonConvert.SerializeObject(item)
                : JsonConvert.SerializeObject(item, settings);

            return cache.SetStringAsync(key, json, options ?? s_DefaultOptions);
        }

        public static TItem GetOrSetAsJsonString<TItem>(this IDistributedCache cache, string key, Func<TItem> factory, DistributedCacheEntryOptions options = null, JsonSerializerSettings settings = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            var (itemFound, cacheItem) = cache.TryGetFromJsonString<TItem>(key);

            if (itemFound)
                return cacheItem;

            // If we get this far then we haven't found the cached item
            TItem createdItem = factory();

            if (createdItem != null)
                SetAsJsonString(cache, key, createdItem, options, settings);

            return createdItem;
        }

        public static async Task<TItem> GetOrSetAsJsonStringAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> factory, DistributedCacheEntryOptions options = null, JsonSerializerSettings settings = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            var (itemFound, cacheItem) = await cache.TryGetFromJsonStringAsync<TItem>(key);

            if (itemFound)
                return cacheItem;

            // If we get this far then we haven't found the cached item
            TItem createdItem = await factory();

            if (createdItem != null)
                await SetAsJsonStringAsync(cache, key, createdItem, options, settings);

            return createdItem;
        }
        #endregion
    }
}
