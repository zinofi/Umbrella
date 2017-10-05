using System;

namespace Umbrella.Utilities
{
    public static class UmbrellaStatics
    {
        public delegate string SerializeJsonDelegate(object value, bool useCamelCasingRules);
        public delegate object DeserializeJsonDelegate(string value);

        public static SerializeJsonDelegate JsonSerializer { private get; set; }

        public static DeserializeJsonDelegate JsonDeserializer { private get; set; }

        public static string SerializeJson(object value, bool useCamelCasingRules = false)
        {
            if(JsonSerializer == null)
                throw new Exception("The JsonSerializer has not been assigned. This should be done on application startup.");

            return JsonSerializer(value, useCamelCasingRules);
        }

        public static T DeserializeJson<T>(string value)
        {
            if(JsonDeserializer == null)
                throw new Exception("The JsonDeserializer has not been assigned. This should be done on application startup.");

            return (T)JsonDeserializer(value);
        }
    }
}