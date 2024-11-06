using System.Runtime.InteropServices;

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace Umbrella.Utilities.Primitives;

/// <summary>
/// Defines a key/value pair that can be set or retrieved.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <remarks>
/// This custom type should be used in place of the <see cref="KeyValuePair{TKey, TValue}"/>
/// type when it is required to be serialized or deserialized using the <see cref="System.Text.Json.JsonSerializer"/>.
/// </remarks>
/// <param name="Key">The key of the key/value pair.</param>
/// <param name="Value">The value of the key/value pair.</param>
[StructLayout(LayoutKind.Auto)]
public readonly record struct UmbrellaKeyValuePair<TKey, TValue>(TKey Key, TValue Value);

#if NET8_0_OR_GREATER
/// <summary>
/// A <see cref="JsonSerializerContext" /> for use with the <see cref="UmbrellaKeyValuePair{TKey, TValue}"/> type.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(UmbrellaKeyValuePair<,>))]
[JsonSerializable(typeof(List<UmbrellaKeyValuePair<string, string>>))]
public partial class UmbrellaKeyValuePairJsonSerializerContext : JsonSerializerContext
{
}
#endif