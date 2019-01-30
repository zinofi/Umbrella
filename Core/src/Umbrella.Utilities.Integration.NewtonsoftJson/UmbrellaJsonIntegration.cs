using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;

namespace Umbrella.Utilities.Integration.NewtonsoftJson
{
    /// <summary>
    /// Provides functionality to integrate the Newtonsoft.Json library with the Umbrella packages.
    /// </summary>
    internal static class UmbrellaJsonIntegration
    {
        private static readonly ConcurrentDictionary<JsonSettingsCacheKey, JsonSerializerSettings> s_SettingsCache = new ConcurrentDictionary<JsonSettingsCacheKey, JsonSerializerSettings>();

        /// <summary>
        /// Initializes the integration.
        /// </summary>
        public static void Initialize()
        {
            JsonSerializerSettings CreateSettings(JsonSettingsCacheKey key)
            {
                var cacheEntry = new JsonSerializerSettings
                {
                    TypeNameHandling = (TypeNameHandling)key.TypeNameHandling,
                };

                if (key.UseCamelCase)
                    cacheEntry.ContractResolver = new CamelCasePropertyNamesContractResolver();

                return cacheEntry;
            }

            UmbrellaStatics.JsonSerializer = (obj, useCamelCase, typeNameHandling) =>
            {
                var jsonSettings = s_SettingsCache.GetOrAdd(new JsonSettingsCacheKey(useCamelCase, typeNameHandling), CreateSettings);

                return JsonConvert.SerializeObject(obj, jsonSettings);
            };

            UmbrellaStatics.JsonDeserializer = (json, type, typeNameHandling) =>
            {
                var jsonSettings = s_SettingsCache.GetOrAdd(new JsonSettingsCacheKey(false, typeNameHandling), CreateSettings);

                return JsonConvert.DeserializeObject(json, type, jsonSettings);
            };
        }
    }
}