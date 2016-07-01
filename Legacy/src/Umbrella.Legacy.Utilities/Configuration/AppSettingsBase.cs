using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;

namespace Umbrella.Legacy.Utilities.Configuration
{
    public abstract class AppSettingsBase
    {
        private static readonly MemoryCacheEntryOptions s_DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();

        private readonly IMemoryCache m_Cache;

        public AppSettingsBase(IMemoryCache cache)
        {
            m_Cache = cache;
        }

        #region Protected Methods
        protected abstract NameValueCollection GetConfigurationSettings();
        protected virtual string GenerateCacheKey(string settingKey) => settingKey;
        protected virtual Func<MemoryCacheEntryOptions> GetCacheEntryOptionsFunc() => null;

        protected T GetSetting<T>(T fallback = default(T), [CallerMemberName]string key = "", bool useCache = true, bool throwException = false)
        {
            return useCache
                ? m_Cache.GetOrCreate(GetType().FullName + ":" + GenerateCacheKey(key), entry =>
                {
                    entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                    return GetSetting(fallback, key, throwException);
                })
                : GetSetting(fallback, key, throwException);
        }

        protected T GetSettingEnum<T>(T fallback = default(T), [CallerMemberName]string key = "", bool useCache = true, bool throwException = false) where T : struct
        {
            return useCache
                ? m_Cache.GetOrCreate(GetType().FullName + ":" + GenerateCacheKey(key), entry =>
                {
                    entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                    return GetSettingEnum(fallback, key, throwException);
                })
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
            if (!string.IsNullOrEmpty(value) && typeof(Enum).IsAssignableFrom(typeof(T)) && Enum.TryParse(value, true, out output))
                return output;
            else if (throwException)
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An appSetting with that key cannot be found", key));

            return fallback;
        }
        #endregion
    }
}