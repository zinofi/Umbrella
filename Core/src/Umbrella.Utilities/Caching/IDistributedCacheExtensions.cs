using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    public static class IDistributedCacheExtensions
    {
        private static readonly DistributedCacheEntryOptions s_DefaultOptions = new DistributedCacheEntryOptions();

        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<Task<string>> factory, DistributedCacheEntryOptions options = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);
            
            if (string.IsNullOrWhiteSpace(result))
            {
                result = await factory();
                await cache.SetStringAsync(key, result, options ?? s_DefaultOptions);
            }

            return result;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> factory, DistributedCacheEntryOptions options = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string result = await cache.GetStringAsync(key);

            if (!string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    TItem item = JsonConvert.DeserializeObject<TItem>(result);

                    return item;
                }
                catch(Exception)
                {
                    await cache.RemoveAsync(key);
                }
            }

            // If we get this far then we haven't found the cached item
            TItem createdItem = await factory();

            if(createdItem != null)
            {
                string json = JsonConvert.SerializeObject(createdItem);

                await cache.SetStringAsync(key, json, options ?? s_DefaultOptions);
            }

            return createdItem;
        }
    }
}