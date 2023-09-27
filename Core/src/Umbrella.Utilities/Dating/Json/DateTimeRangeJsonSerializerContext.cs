using System.Text.Json.Serialization;

namespace Umbrella.Utilities.Dating.Json;

/// <summary>
/// A <see cref="JsonSerializerContext" /> for use with the <see cref="Dating.DateTimeRange"/> type.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DateTimeRange))]
public partial class DateTimeRangeJsonSerializerContext : JsonSerializerContext
{
}