using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Configuration
{
    public abstract class ReadOnlyAppSettingsBase : ReadOnlyAppSettingsBase<IReadOnlyAppSettingsSource>
    {
        #region Constructors
        public ReadOnlyAppSettingsBase(ILogger logger,
            IMemoryCache cache,
            IReadOnlyAppSettingsSource appSettingsSource)
            : base(logger, cache, appSettingsSource)
        {
        } 
        #endregion
    }

    public abstract class ReadOnlyAppSettingsBase<TAppSettingsSource>
        where TAppSettingsSource : IReadOnlyAppSettingsSource
    {
        #region Private Static Members
        private static readonly MemoryCacheEntryOptions s_DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();
        private static readonly string s_CacheKeyPrefix = typeof(ReadOnlyAppSettingsBase<TAppSettingsSource>).FullName;
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IMemoryCache Cache { get; }
        protected TAppSettingsSource AppSettingsSource { get; }
        #endregion

        #region Constructors
        public ReadOnlyAppSettingsBase(ILogger logger,
            IMemoryCache cache,
            TAppSettingsSource appSettingsSource)
        {
            Log = logger;
            Cache = cache;
            AppSettingsSource = appSettingsSource;
        }
        #endregion

        #region Protected Methods
        protected virtual string GenerateCacheKey(string settingKey) => $"{s_CacheKeyPrefix}:{settingKey}";
        protected virtual Func<MemoryCacheEntryOptions> GetCacheEntryOptionsFunc() => null;
        protected abstract T FromJson<T>(string value);

        protected virtual T GetSetting<T>(T fallback = default, [CallerMemberName]string key = "", bool useCache = true, bool throwException = false)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

                return useCache
                    ? Cache.GetOrCreate(GenerateCacheKey(key), entry =>
                    {
                        entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                        return GetSetting(fallback, key, throwException);
                    })
                    : GetSetting(fallback, key, throwException);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        protected virtual T GetSettingEnum<T>(T fallback = default, [CallerMemberName]string key = "", bool useCache = true, bool throwException = false) where T : struct
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

                return useCache
                    ? Cache.GetOrCreate(GenerateCacheKey(key), entry =>
                    {
                        entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                        return GetSettingEnum(fallback, key, throwException);
                    })
                    : GetSettingEnum(fallback, key, throwException);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion

        #region Private Methods
        private T GetSetting<T>(T fallback = default, [CallerMemberName]string key = "", bool throwException = false)
        {
            string value = AppSettingsSource.GetValue(key);

            var type = typeof(T);

            if (!string.IsNullOrEmpty(value))
            {
                if (type.GetTypeInfo().IsPrimitive || type == typeof(string))
                    return (T)Convert.ChangeType(value, type);

                return FromJson<T>(value);
            }
            else if (throwException)
            {
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An app setting with that key cannot be found", key));
            }

            return type == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, type)
                : fallback;
        }

        private T GetSettingEnum<T>(T fallback = default, [CallerMemberName]string key = "", bool throwException = false) where T : struct
        {
            string value = AppSettingsSource.GetValue(key);

            if (!string.IsNullOrEmpty(value) && typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) && Enum.TryParse(value, true, out T output))
                return output;
            else if (throwException)
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An enum app setting with that key cannot be found", key));

            return fallback;
        }
        #endregion
    }
}