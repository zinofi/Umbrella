using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess
{
    public class CacheEntry<TItem>
    {
        #region Public Properties
        public TItem Item { get; set; }
        public Dictionary<string, object> MetaData { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Constructors
        public CacheEntry()
        {
        }

        public CacheEntry(TItem item)
        {
            Item = item;
        }
        #endregion

        #region Public Methods
        public T GetMetaDataObjectEntry<T>(string key)
        {
            object entry;
            if (MetaData.TryGetValue(key, out entry))
            {
                if (entry != null)
                    return (T)entry;
            }

            return default(T);
        }

        public List<T> GetMetaDataListEntry<T>(string key)
        {
            List<object> lstObject = GetMetaDataObjectEntry<List<object>>(key);

            return lstObject != null ? lstObject.Cast<T>().ToList() : null;
        }
        #endregion
    }
}