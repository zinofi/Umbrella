using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Caching
{
    public class SynchronizedCache<T, U>
    {
        #region Private Static Members
        //TODO: private static readonly ILog Log = LogManager.GetLogger(typeof(SynchronizedCache<T, U>));
        private readonly Dictionary<T, U> s_Cache = new Dictionary<T, U>();
        private readonly ReaderWriterLockSlim s_Lock = new ReaderWriterLockSlim();
        #endregion

        #region Public Methods
        public U Get(T key)
        {
            s_Lock.EnterReadLock();

            try
            {
                if (s_Cache.ContainsKey(key))
                    return s_Cache[key];
            }
            catch (Exception exc) //TODO: when (Log.LogError(exc, key))
            {
                throw;
            }
            finally
            {
                s_Lock.ExitReadLock();
            }

            return default(U);
        }
        
        public void Add(T key, U value)
        {
            try
            {
                s_Lock.EnterUpgradeableReadLock();

                if (!s_Cache.ContainsKey(key))
                {
                    s_Lock.EnterWriteLock();

                    if (!s_Cache.ContainsKey(key))
                    {
                        s_Cache.Add(key, value);
                    }

                    s_Lock.ExitWriteLock();
                }
            }
            catch (Exception exc) //TODO: when (Log.LogError(exc, new { key = key, value = value }))
            {
                throw;
            }
            finally
            {
                s_Lock.ExitUpgradeableReadLock();
            }
        }

        public void Clear()
        {
            s_Lock.EnterWriteLock();

            try
            {
                s_Cache.Clear();
            }
            catch(Exception exc) //TODO: when (Log.LogError(exc))
            {
                throw;
            }
            finally
            {
                s_Lock.ExitWriteLock();
            }
        }
        #endregion
    }
}