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
        }
        #endregion

        #region Public Methods
        public TMetaData GetMetaDataEntry<TMetaData>(string key)
        {
            object entry;
            if (MetaData.TryGetValue(key, out entry))
            {
                if (entry != null)
                    return (TMetaData)entry;
            }

            return default(TMetaData);
        }
        #endregion
    }
}