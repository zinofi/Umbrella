using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;

namespace Umbrella.N2.Utilities.Caching
{
    public class N2MemoryCache<T, U> : MemoryCache<T, U>
    {
        private static readonly N2MemoryCache<T, U> s_Cache = new N2MemoryCache<T, U>();

        /// <summary>
        /// The global cache instance
        /// </summary>
        public static new N2MemoryCache<T, U> Global
        {
            get { return s_Cache; }
        }

        public N2MemoryCache(Func<CacheItemPolicy> defaultPolicyFunc = null)
            : base(defaultPolicyFunc)
        {
            global::N2.Context.Persister.ItemSaved += Persister_ItemChanged;
            global::N2.Context.Persister.ItemDeleted += Persister_ItemChanged;
        }

        private void Persister_ItemChanged(object sender, global::N2.ItemEventArgs e)
        {
			p_Cache.Clear();
        }
    }
}