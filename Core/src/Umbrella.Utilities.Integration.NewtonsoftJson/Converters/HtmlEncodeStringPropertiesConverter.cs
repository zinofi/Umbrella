using System;
using System.Net;
using Newtonsoft.Json;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Integration.NewtonsoftJson.Converters
{
	/// <summary>
	/// A custom <see cref="JsonConverter"/> to ensure string values are properly encoded and decoded to mitigate XSS attacks.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	public class HtmlEncodeStringPropertiesConverter : JsonConverter
	{
		#region Overridden Methods		
		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType) => objectType == typeof(string);

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		/// <exception cref="ArgumentNullException">reader</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string value = reader.Value as string;

			if (!string.IsNullOrEmpty(value))
			{
				// Clean the incoming string before running through the sanitizer and then again afterwards
				// to prevent double encoding of things we don't want encoding.
				value = WebUtility.HtmlEncode(value.Clean()).Clean();
			}

			return value;
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string valueToWrite = value as string;

			if (!string.IsNullOrEmpty(valueToWrite))
			{
				// Cleaning again on the way out to ensure that anything that is encoded
				// that shouldn't be is decoded properly.
				valueToWrite = valueToWrite.Clean();
			}

			writer.WriteValue(valueToWrite ?? string.Empty);
		}
		#endregion
	}
}