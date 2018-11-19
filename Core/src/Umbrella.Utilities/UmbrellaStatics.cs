using System;
using Umbrella.Utilities.Json;

namespace Umbrella.Utilities
{
    public static class UmbrellaStatics
    {
        public delegate string SerializeJsonDelegate(object value, bool useCamelCasingRules, TypeNameHandling typeNameHandling);
        public delegate object DeserializeJsonDelegate(string value, Type type, TypeNameHandling typeNameHandling);

        public static SerializeJsonDelegate JsonSerializer { private get; set; }
        public static DeserializeJsonDelegate JsonDeserializer { private get; set; }

        public static string SerializeJson(object value, bool useCamelCasingRules = false, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            if(JsonSerializer == null)
                throw new Exception("The JsonSerializer has not been assigned. This should be done on application startup.");

            return JsonSerializer(value, useCamelCasingRules, typeNameHandling);
        }

        public static T DeserializeJson<T>(string value, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            Guard.ArgumentNotNullOrWhiteSpace(value, nameof(value));

            if(JsonDeserializer == null)
                throw new Exception("The JsonDeserializer has not been assigned. This should be done on application startup.");

            return (T)JsonDeserializer(value, typeof(T), typeNameHandling);
        }
    }
}