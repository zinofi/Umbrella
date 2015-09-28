using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;

namespace Umbrella.N2.Utilities.Caching
{
    public class N2Cache<T, U> : Cache<T, U>
    {
        private static readonly N2Cache<T, U> s_Cache = new N2Cache<T, U>();

        /// <summary>
        /// The global cache instance
        /// </summary>
        public static N2Cache<T, U> Global
        {
            get { return s_Cache; }
        }

        public N2Cache()
        {
            global::N2.Context.Persister.ItemSaved += Persister_ItemChanged;
            global::N2.Context.Persister.ItemDeleted += Persister_ItemChanged;
        }

        private void Persister_ItemChanged(object sender, global::N2.ItemEventArgs e)
        {
            lock (p_Lock)
                p_Cache = new Dictionary<T, U>();
        }
    }
}