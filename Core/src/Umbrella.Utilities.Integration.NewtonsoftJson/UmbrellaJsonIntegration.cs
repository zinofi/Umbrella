using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbrella.Utilities.Integration.NewtonsoftJson
{
	/// <summary>
	/// Provides functionality to integrate the Newtonsoft.Json library with the Umbrella packages.
	/// </summary>
	internal static class UmbrellaJsonIntegration
	{
		private static readonly JsonSerializerSettings _camelCaseSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

		/// <summary>
		/// Initializes the integration.
		/// </summary>
		public static void Initialize()
		{
			UmbrellaStatics.JsonSerializerImplementation = (obj, useCamelCase) => JsonConvert.SerializeObject(obj, useCamelCase ? _camelCaseSettings : null);
			UmbrellaStatics.JsonDeserializerImplementation = (json, type) => JsonConvert.DeserializeObject(json, type);
		}
	}
}