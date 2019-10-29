using System;
using System.Net;
using Newtonsoft.Json;

namespace Umbrella.Utilities.Integration.NewtonsoftJson.Converters
{
	/// <summary>
	/// A custom <see cref="JsonConverter"/> to ensure string values are properly encoded when they are output to mitigate XSS attacks.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	/// <remarks>
	/// This has been altered as of v3 so that nothing is encoded on the way in as per Microsoft's advice: https://docs.microsoft.com/en-us/aspnet/core/security/cross-site-scripting
	/// The general accepted practice is that encoding takes place at the point of output and encoded values should never be stored in a database.
	/// Encoding at the point of output allows you to change the use of data, for example, from HTML to a query string value.
	/// It also enables you to easily search your data without having to encode values before searching and allows you to take advantage of any changes or bug fixes made to encoders.
	/// </remarks>
	public class HtmlEncodeStringPropertiesConverter : JsonConverter
	{
		#region Static Delegates
		/// <summary>
		/// Gets or sets the optional delegate invoked at the end of the <see cref="ReadJson(JsonReader, Type, object, JsonSerializer)"/> method immediately before returning.
		/// </summary>
		public static Func<JsonReader, Type, JsonSerializer, string, string> OnReadJson { get; set; }

		/// <summary>
		/// Gets or sets the optional delegate invoked at the end of the <see cref="WriteJson(JsonWriter, object, JsonSerializer)"/> method immediately before returning.
		/// </summary>
		public static Func<JsonWriter, JsonSerializer, string, string> OnWriteJson { get; set; }
		#endregion

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
			string valueToRead = reader.Value as string;

			OnReadJson?.Invoke(reader, objectType, serializer, valueToRead);

			return valueToRead;
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
				// Always encode on the way out and leave it to the client to decide on what to do.
				valueToWrite = WebUtility.HtmlEncode(valueToWrite);
			}

			OnWriteJson?.Invoke(writer, serializer, valueToWrite);

			writer.WriteValue(valueToWrite);
		}
		#endregion
	}
}