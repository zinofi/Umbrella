using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    //TODO: This needs some work. We need to add in an optional locking mechanism and also investigate performing binary serialization, although that might not always be possible
    //if types have to be annotated by the serialization mechanism. For the JSON, we need to specify settings that ensure the type names are always serialized properly.
    //Also, ensure the cache entry options are always provided. We don't want any kind of defaults for these.
    //Might also be a good idea to incorporate the .NET type name into the Keys.
    public static class IDistributedCacheExtensions
    {
        #region Private Static Members
        private static JsonSerializerSettings s_JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        #endregion

        #region Public Static Members
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
        public static string GetOrCreateString(this IDistributedCache cache, string key, Func<string> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            
            string result = cache.GetString(key);
            
            if (string.IsNullOrWhiteSpace(result))
            {
                result = factory();

                if (!string.IsNullOrWhiteSpace(result))
                    cache.SetString(key, result, options);
            }

            return result;
        }

        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<Task<string>> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = await factory();

                if (!string.IsNullOrWhiteSpace(result))
                    await cache.SetStringAsync(key, result, options);
            }

            return result;
        }

        public static (bool itemFound, TItem cacheItem) TryGet<TItem>(this IDistributedCache cache, string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = cache.GetString(key);

            if (!string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    var item = JsonConvert.DeserializeObject<TItem>(result, s_JsonSerializerSettings);

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

        public static async Task<(bool itemFound, TItem cacheItem)> TryGetAsync<TItem>(this IDistributedCache cache, string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);

            if (!string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    var item = JsonConvert.DeserializeObject<TItem>(result, s_JsonSerializerSettings);

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

        public static TItem Get<TItem>(this IDistributedCache cache, string key) => cache.TryGet<TItem>(key).cacheItem;
        public static async Task<TItem> GetAsync<TItem>(this IDistributedCache cache, string key) => (await cache.TryGetAsync<TItem>(key)).cacheItem;

        public static void Set(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(item, nameof(item));
            
            string json = JsonConvert.SerializeObject(item, s_JsonSerializerSettings);

            cache.SetString(key, json, options);
        }

        public static Task SetAsync(this IDistributedCache cache, string key, object item, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNull(item, nameof(item));

            string json = JsonConvert.SerializeObject(item, s_JsonSerializerSettings);

            return cache.SetStringAsync(key, json, options);
        }

        public static TItem GetOrCreate<TItem>(this IDistributedCache cache, string key, Func<TItem> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            var (itemFound, cacheItem) = cache.TryGet<TItem>(key);

            if (itemFound)
                return cacheItem;

            // If we get this far then we haven't found the cached item
            TItem createdItem = factory();

            if (createdItem != null)
                Set(cache, key, createdItem, options);

            return createdItem;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> factory, DistributedCacheEntryOptions options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            var (itemFound, cacheItem) = await cache.TryGetAsync<TItem>(key);

            if (itemFound)
                return cacheItem;

            // If we get this far then we haven't found the cached item
            TItem createdItem = await factory();

            if (createdItem != null)
                await SetAsync(cache, key, createdItem, options);

            return createdItem;
        }
        #endregion
    }
}