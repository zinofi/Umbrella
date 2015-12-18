using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Caching
{
    /// <summary>
    /// A generic cache which internally uses a dictionary to store data. Data in this cache is never evicted.
    /// </summary>
    /// <typeparam name="T">The type of the key</typeparam>
    /// <typeparam name="U">The type of the item being stored</typeparam>
    public class Cache<T, U>
    {
        //TODO: private static readonly ILog Log = LogManager.GetLogger(typeof(Cache<T, U>));
        private static readonly Cache<T, U> s_Cache = new Cache<T, U>();

        protected readonly object p_Lock = new object();
        protected Dictionary<T, U> p_Cache = new Dictionary<T, U>();

        /// <summary>
        /// The global cache instance
        /// </summary>
        public static Cache<T, U> Global
        {
            get { return s_Cache; }
        }

        /// <summary>
        /// Initialize a new instance of Cache
        /// </summary>
        public Cache()
        {
        }

        public U Get(T key)
        {
            if (p_Cache.ContainsKey(key))
                return p_Cache[key];

            return default(U);
        }

        /// <summary>
        /// Used to retrieve an item from the cache or add it to the cache if it doesn't exist
        /// using the specified key.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="adder">A function used to add a new item into the cache using the specified key</param>
        /// <returns>Returns the item from the cache</returns>
        public U AddOrGet(T key, Func<U> adder)
        {
            try
            {
                if (!p_Cache.ContainsKey(key))
                {
                    //Invoke the adder() delegate here outside of the lock
                    //If the method takes a long time to execute, there could potentially
                    //be many threads waiting on the lock if this cache is being used
                    //by a high volume of threads
                    U result = adder();

                    //Recheck if the item is in the dictionary - another thread
                    //may have beaten us to it
                    if (!p_Cache.ContainsKey(key))
                    {
                        lock (p_Lock)
                        {
                            if (!p_Cache.ContainsKey(key))
                            {
                                p_Cache.Add(key, result);
                            }
                        }
                    }
                }

                return p_Cache[key];
            }
            catch (Exception exc) //TODO: when (Log.LogError(exc))
            {
                throw;
            }
        }

        /// <summary>
        /// Removes the item with the specifed key from the dictionary
        /// </summary>
        /// <param name="key">The key of the item to remove from the dictionary</param>
        public void Remove(T key)
        {
            p_Cache.Remove(key);
        }
    }
}