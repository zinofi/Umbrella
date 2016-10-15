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
        private static readonly ConcurrentDictionary<int, string> s_CacheKeyDictionary = new ConcurrentDictionary<int, string>();
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
        public object Get(int id) => m_Cache.Get(GenerateCacheKey(id));
        public TItem Get<TItem>(int id) => m_Cache.Get<TItem>(GenerateCacheKey(id));
        public TItem GetOrCreate<TItem>(int id, Func<ICacheEntry, TItem> factory) => m_Cache.GetOrCreate(GenerateCacheKey(id), factory);
        public Task<TItem> GetOrCreateAsync<TItem>(int id, Func<ICacheEntry, Task<TItem>> factory) => m_Cache.GetOrCreateAsync(GenerateCacheKey(id), factory);
        public void Set<TItem>(int id, TItem value, MemoryCacheEntryOptions options = null) => m_Cache.Set(GenerateCacheKey(id), value, options ?? s_DefaultMemoryCacheEntryOptions);
        public bool TryGetValue(int id, out object value) => m_Cache.TryGetValue(GenerateCacheKey(id), out value);
        public bool TryGetValue<TItem>(int id, out TItem value) => m_Cache.TryGetValue(GenerateCacheKey(id), out value);
        #endregion

        #region Event Handlers
        private void Persister_ItemChanged(object sender, ItemEventArgs e)
        {
            int id = e.AffectedItem.ID;

            string cacheKey = GenerateCacheKey(id);

            m_Cache.Remove(cacheKey);

            if (e.AffectedItem.State == ContentState.Deleted)
            {
                string value;
                s_CacheKeyDictionary.TryRemove(id, out value);
            }
        }
        #endregion

        #region Private Methods
        private string GenerateCacheKey(int id) => s_CacheKeyDictionary.GetOrAdd(id, x => $"{s_CacheKeyPrefix}:{x}"); 
        #endregion
    }
}