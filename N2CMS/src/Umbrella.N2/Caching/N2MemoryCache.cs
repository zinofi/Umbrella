using Microsoft.Extensions.Caching.Memory;
using N2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.N2.Caching
{
    public class N2MemoryCache
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(N2MemoryCache).FullName;
        private static readonly MemoryCacheEntryOptions s_DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();
        private static readonly ConcurrentDictionary<int, ConcurrentBag<string>> s_CacheKeyDictionary = new ConcurrentDictionary<int, ConcurrentBag<string>>();
        #endregion

        #region Private Members
        private readonly IMemoryCache m_Cache;
        #endregion

        #region Constructors
        public N2MemoryCache(IMemoryCache memoryCache)
        {
            m_Cache = memoryCache;

            Context.Persister.ItemSaved += Persister_ItemChanged;
            Context.Persister.ItemDeleted += Persister_ItemChanged;
        }
        #endregion

        #region Public Methods
        public object Get(int contentItemId, string key) => m_Cache.Get(GenerateCacheKey(contentItemId, key));
        public TItem Get<TItem>(int contentItemId, string key) => m_Cache.Get<TItem>(GenerateCacheKey(contentItemId, key));
        public TItem GetOrCreate<TItem>(int contentItemid, string key, Func<ICacheEntry, TItem> factory) => m_Cache.GetOrCreate(GenerateCacheKey(contentItemid, key), factory);
        public Task<TItem> GetOrCreateAsync<TItem>(int contentItemid, string key, Func<ICacheEntry, Task<TItem>> factory) => m_Cache.GetOrCreateAsync(GenerateCacheKey(contentItemid, key), factory);
        public void Set<TItem>(int contentItemid, string key, TItem value, MemoryCacheEntryOptions options = null) => m_Cache.Set(GenerateCacheKey(contentItemid, key), value, options ?? s_DefaultMemoryCacheEntryOptions);
        public bool TryGetValue(int contentItemid, string key, out object value) => m_Cache.TryGetValue(GenerateCacheKey(contentItemid, key), out value);
        public bool TryGetValue<TItem>(int contentItemid, string key, out TItem value) => m_Cache.TryGetValue(GenerateCacheKey(contentItemid, key), out value);
        #endregion

        #region Event Handlers
        private void Persister_ItemChanged(object sender, ItemEventArgs e)
        {
            int id = e.AffectedItem.ID;

            ConcurrentBag<string> lstCacheKey;
            if (s_CacheKeyDictionary.TryGetValue(id, out lstCacheKey))
            {
                foreach(string cacheKey in lstCacheKey)
                {
                    m_Cache.Remove(cacheKey);
                }

                if (e.AffectedItem.State == ContentState.Deleted)
                {
                    ConcurrentBag<string> value;
                    s_CacheKeyDictionary.TryRemove(id, out value);
                }
            }
        }
        #endregion

        #region Private Methods
        private string GenerateCacheKey(int contentItemId, string key)
        {
            var cacheKey = $"{s_CacheKeyPrefix}:{contentItemId}:{key}";

            var keys = s_CacheKeyDictionary.GetOrAdd(contentItemId, new ConcurrentBag<string>());
            keys.Add(cacheKey);

            return cacheKey;
        }
        #endregion
    }
}