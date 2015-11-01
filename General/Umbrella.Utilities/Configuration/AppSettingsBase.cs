using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;

namespace Umbrella.Utilities.Configuration
{
    public abstract class AppSettingsBase
    {
        #region Private Static Members
        private static readonly MemoryCache<string, object> s_Cache = new MemoryCache<string, object>();
        #endregion

        #region Protected Methods
        protected abstract NameValueCollection GetConfigurationSettings();
        protected virtual string GenerateCacheKey(string settingKey) => settingKey;
        protected virtual Func<CacheItemPolicy> GetCacheItemPolicyFunc() => null;

        protected T GetSetting<T>(T fallback = default(T), [CallerMemberName]string key = "", bool useCache = true, bool throwException = false)
        {
            return useCache
                ? (T)s_Cache.AddOrGet(GetType().FullName + ":" + GenerateCacheKey(key), () => GetSetting(fallback, key, throwException), GetCacheItemPolicyFunc())
                : GetSetting(fallback, key, throwException);
        }

        protected T GetSettingEnum<T>(T fallback = default(T), [CallerMemberName]string key = "", bool useCache = true, bool throwException = false) where T : struct
        {
            return useCache
                ? (T)s_Cache.AddOrGet(GetType().FullName + ":" + GenerateCacheKey(key), () => GetSettingEnum(fallback, key, throwException), GetCacheItemPolicyFunc())
                : GetSettingEnum(fallback, key, throwException);
        }
        #endregion

        #region Private Methods
        private T GetSetting<T>(T fallback = default(T), [CallerMemberName]string key = "", bool throwException = false)
        {
            NameValueCollection settings = GetConfigurationSettings();

            if (settings == null)
                throw new InvalidOperationException("The configuration settings are null. Check the GetConfigurationSettings is correct.");

            string value = settings[key];

            if (!string.IsNullOrEmpty(value))
                return (T)Convert.ChangeType(value, typeof(T));
            else if (throwException)
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An appSetting with that key cannot be found", key));

            return typeof(T) == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, typeof(T))
                : fallback;
        }

        private T GetSettingEnum<T>(T fallback = default(T), [CallerMemberName]string key = "", bool throwException = false) where T : struct
        {
            NameValueCollection settings = GetConfigurationSettings();

            if (settings == null)
                throw new InvalidOperationException("The configuration settings are null. Check the GetConfigurationSettings is correct.");

            string value = settings[key];

            T output;
            if (!string.IsNullOrEmpty(value) && typeof(Enum).IsAssignableFrom(typeof(T)) && Enum.TryParse<T>(value, true, out output))
                return output;
            else if (throwException)
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An appSetting with that key cannot be found", key));

            return fallback;
        }
        #endregion
    }
}