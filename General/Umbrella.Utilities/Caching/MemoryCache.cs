using System;
using System.Runtime.Caching;
using Umbrella.Utilities.Extensions;
using System.Collections.Concurrent;

namespace Umbrella.Utilities.Caching
{
    /// <summary>
    /// A generic wrapper for the System.Runtime.Caching.MemoryCache class.
    /// </summary>
    /// <typeparam name="T">The type of the key</typeparam>
    /// <typeparam name="U">The type of the item being stored</typeparam>
    public class MemoryCache<T, U>
    {
		#region Private Static Members
		//TODO: private static readonly ILog Log = LogManager.GetLogger(typeof(MemoryCache<T, U>));
		private static readonly MemoryCache<T, U> s_Cache = new MemoryCache<T, U>();
		#endregion

		#region Private Members
		private readonly Func<CacheItemPolicy> m_DefaultCacheItemPolicyFunc = null;
		#endregion

		#region Protected Members
		protected readonly ConcurrentDictionary<Type, MemoryCache> p_Cache = new ConcurrentDictionary<Type, MemoryCache>();
		#endregion

		#region Public Static Properties
		/// <summary>
		/// The global cache instance
		/// </summary>
		public static MemoryCache<T, U> Global
		{
			get { return s_Cache; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of MemoryCache
		/// </summary>
		public MemoryCache(Func<CacheItemPolicy> defaultPolicyFunc = null)
		{
            m_DefaultCacheItemPolicyFunc = defaultPolicyFunc ?? (() => new CacheItemPolicy());
		}
		#endregion

		#region Public Methods
		public U Get(T key)
		{
			try
			{
				Type type = typeof(U);

				MemoryCache cache;

				if (p_Cache.TryGetValue(type, out cache))
				{
					string strKey = key.ToString();

					object objResult = cache[strKey];

					if (objResult != null)
						return (U)objResult;
				}

				return default(U);
			}
			catch (Exception exc) //TODO: when (Log.LogError(exc, key))
			{
				throw;
			}
		}

		/// <summary>
		/// Used to retrieve an item from the cache or add it to the cache if it doesn't exist
		/// using the specified key.
		/// </summary>
		/// <param name="key">The cache key</param>
		/// <param name="adder">A function used to add a new item into the cache using the specified key</param>
		/// <returns>Returns the item from the cache</returns>
		public U AddOrGet(T key, Func<U> adder, Func<CacheItemPolicy> policyFunc = null)
		{
			try
			{
				MemoryCache cache = p_Cache.GetOrAdd(typeof(U), x => new MemoryCache(x.FullName));

				string strKey = key.ToString();

				object objResult = cache[strKey];

				if (objResult != null)
					return (U)objResult;

				U result = adder();

				if (result != null)
					cache.Set(strKey, result, policyFunc != null ? policyFunc() : m_DefaultCacheItemPolicyFunc());

				return result;
			}
			catch (Exception exc) //TODO: when (Log.LogError(exc, key))
			{
				throw;
			}
		}

		public void Remove(T key)
		{
			try
			{
				MemoryCache cache;

				if (p_Cache.TryGetValue(typeof(U), out cache))
					cache.Remove(key.ToString());
			}
			catch (Exception exc) //TODO: when (Log.LogError(exc, key))
			{
				throw;
			}
		}
		#endregion
    }
}