using System;
using System.Runtime.InteropServices;
using Umbrella.Utilities.Json;

namespace Umbrella.Utilities.Integration.NewtonsoftJson
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct JsonSettingsCacheKey : IEquatable<JsonSettingsCacheKey>
    {
        public JsonSettingsCacheKey(bool useCamelCase, UmbrellaJsonTypeNameHandling typeNameHandling)
        {
            UseCamelCase = useCamelCase;
            TypeNameHandling = typeNameHandling;
        }

        public bool UseCamelCase { get; }
        public UmbrellaJsonTypeNameHandling TypeNameHandling { get; }

        public override bool Equals(object obj)
        {
            return obj is JsonSettingsCacheKey && Equals((JsonSettingsCacheKey)obj);
        }

        public bool Equals(JsonSettingsCacheKey other)
        {
            return UseCamelCase == other.UseCamelCase &&
                   TypeNameHandling == other.TypeNameHandling;
        }

        public override int GetHashCode()
        {
            var hashCode = 1324301641;
            hashCode = hashCode * -1521134295 + UseCamelCase.GetHashCode();
            hashCode = hashCode * -1521134295 + TypeNameHandling.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(JsonSettingsCacheKey key1, JsonSettingsCacheKey key2)
        {
            return key1.Equals(key2);
        }

        public static bool operator !=(JsonSettingsCacheKey key1, JsonSettingsCacheKey key2)
        {
            return !(key1 == key2);
        }
    }
}