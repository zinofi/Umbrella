using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbrella.Utilities.JsonConverters
{
	/// <summary>
	/// A custom converter to allow serialization and deserialization of nullable <see cref="TimeSpan"/> instances.
	/// </summary>
	/// <remarks>
	/// Built in support should be available in .NET 6 after which this will be removed.
	/// </remarks>
	public class NullableTimeSpanConverter : JsonConverter<TimeSpan?>
	{
		/// <inheritdoc />
		public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> TimeSpan.TryParse(reader.GetString(), CultureInfo.InvariantCulture, out TimeSpan result)
				? result
				: (TimeSpan?)null;

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
		{
			string? output = !value.HasValue ? null : value.Value.ToString(format: null, CultureInfo.InvariantCulture);
			writer.WriteStringValue(output);
		}
	}
}