using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.Configuration
{
    //TODO: Could refactor the private methods to be local functions.
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
        protected virtual T FromJson<T>(string value) => UmbrellaStatics.DeserializeJson<T>(value);

        protected virtual T GetSetting<T>(T fallback = default, [CallerMemberName]string key = "", bool useCache = true, bool throwException = false, Func<string, T> customValueConverter = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            try
            {
                return useCache
                    ? Cache.GetOrCreate(GenerateCacheKey(key), entry =>
                    {
                        entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                        return GetSetting(fallback, key, throwException, customValueConverter);
                    })
                    : GetSetting(fallback, key, throwException, customValueConverter);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        protected virtual T GetSetting<T>(Func<T> fallbackCreator = null, [CallerMemberName]string key = "", bool useCache = true, bool throwException = false, Func<string, T> customValueConverter = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            try
            {
                return useCache
                    ? Cache.GetOrCreate(GenerateCacheKey(key), entry =>
                    {
                        entry.SetOptions(GetCacheEntryOptionsFunc()?.Invoke() ?? s_DefaultMemoryCacheEntryOptions);
                        return GetSetting(fallbackCreator, key, throwException, customValueConverter);
                    })
                    : GetSetting(fallbackCreator, key, throwException, customValueConverter);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        protected virtual T GetSettingEnum<T>(T fallback = default, [CallerMemberName]string key = "", bool useCache = true, bool throwException = false)
            where T : struct, Enum
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            try
            {
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
        private T GetSetting<T>(T fallback = default, [CallerMemberName]string key = "", bool throwException = false, Func<string, T> customValueConverter = null)
        {
            string value = AppSettingsSource.GetValue(key);

            var type = typeof(T);

            if (!string.IsNullOrEmpty(value))
                return customValueConverter != null ? customValueConverter(value) : (T)Convert.ChangeType(value, type);
            else if (throwException)
                throw new ArgumentException($"The value for key: {key} is not valid. An app setting with that key cannot be found.");

            return type == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, type)
                : fallback;
        }

        private T GetSetting<T>(Func<T> fallbackCreator = null, [CallerMemberName]string key = "", bool throwException = false, Func<string, T> customValueConverter = null)
        {
            string value = AppSettingsSource.GetValue(key);

            var type = typeof(T);

            if (!string.IsNullOrEmpty(value))
                return customValueConverter != null ? customValueConverter(value) : (T)Convert.ChangeType(value, type);
            else if (throwException)
                throw new ArgumentException($"The value for key: {key} is not valid. An app setting with that key cannot be found.");

            T defaultValue = fallbackCreator != null ? fallbackCreator() : default;

            return type == typeof(string) && defaultValue == null
                ? (T)Convert.ChangeType(string.Empty, type)
                : defaultValue;
        }

        private T GetSettingEnum<T>(T fallback = default, [CallerMemberName]string key = "", bool throwException = false)
            where T : struct, Enum
        {
            string value = AppSettingsSource.GetValue(key);

            if (!string.IsNullOrEmpty(value) && typeof(Enum).IsAssignableFrom(typeof(T)) && Enum.TryParse(value, true, out T output))
                return output;
            else if (throwException)
                throw new ArgumentException($"The value for key: {key} is not valid. An enum app setting with that key cannot be found.");

            return fallback;
        }
        #endregion
    }
}