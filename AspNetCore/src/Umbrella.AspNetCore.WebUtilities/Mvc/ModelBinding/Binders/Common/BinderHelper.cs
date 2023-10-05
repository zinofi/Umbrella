using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.Common;

/// <summary>
/// A helper for use with model binders.
/// </summary>
public static class BinderHelper
{
	/// <summary>
	/// JSON Serializer options commonly used with model binders.
	/// </summary>
	public static readonly JsonSerializerOptions SerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		Converters = {
			new JsonStringEnumConverter()
		}
	};
}