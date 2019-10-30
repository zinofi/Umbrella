using System;
using System.Text.Json;

namespace Umbrella.Utilities
{
    public static class UmbrellaStatics
    {
		private static readonly JsonSerializerOptions _camelCaseOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		private static readonly JsonSerializerOptions _ignoreCaseOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public delegate string SerializeJsonDelegate(object value, bool useCamelCasingRules);
        public delegate object DeserializeJsonDelegate(string value, Type type);

		public static SerializeJsonDelegate JsonSerializerImplementation { private get; set; } = DefaultSerialize;
		public static DeserializeJsonDelegate JsonDeserializerImplementation { private get; set; } = DefaultDeserialize;

        public static string SerializeJson(object value, bool useCamelCasingRules = false)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            if(JsonSerializerImplementation == null)
                throw new Exception("The JsonSerializer has not been assigned. This should be done on application startup.");

            return JsonSerializerImplementation(value, useCamelCasingRules);
        }

        public static T DeserializeJson<T>(string value)
        {
            Guard.ArgumentNotNullOrWhiteSpace(value, nameof(value));

            if(JsonDeserializerImplementation == null)
                throw new Exception("The JsonDeserializer has not been assigned. This should be done on application startup.");

            return (T)JsonDeserializerImplementation(value, typeof(T));
        }

		public static string DefaultSerialize(object value, bool useCamelCasingRules) => JsonSerializer.Serialize(value, value.GetType(), useCamelCasingRules ? _camelCaseOptions : null);

		public static object DefaultDeserialize(string value, Type type) => JsonSerializer.Deserialize(value, type, _ignoreCaseOptions);
    }
}