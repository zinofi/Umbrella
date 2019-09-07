using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Umbrella.DataAccess.EF.Abstractions
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
			if (MetaData.TryGetValue(key, out object entry))
			{
				if (entry != null)
					return (T)entry;
			}

			return default;
		}

		public IReadOnlyCollection<T> GetMetaDataListEntry<T>(string key)
		{
			object lstObject = GetMetaDataObjectEntry<object>(key);

			if (lstObject is List<T> list)
				return list;

			if (lstObject is JArray jArray)
				return jArray.Select(x => x.Value<T>()).ToList();

			return null;
		}
		#endregion
	}
}