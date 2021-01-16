using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbrella.Utilities.JsonConverters
{
	/// <summary>
	/// A custom converter to allow serialization and deserialization of <see cref="TimeSpan"/> instances.
	/// </summary>
	/// <remarks>
	/// Built in support should be available in .NET 6 after which this will be removed.
	/// </remarks>
	public class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		/// <inheritdoc />
		public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> TimeSpan.Parse(reader.GetString(), CultureInfo.InvariantCulture);

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(format: null, CultureInfo.InvariantCulture));
	}
}