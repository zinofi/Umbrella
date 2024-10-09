using Microsoft.AspNetCore.Mvc;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.JsonArray;

/// <summary>
/// An attribute that can be applied to a model, property or action method parameter to specify that the <see cref="JsonArrayModelBinder"/> should be used to bind the model.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class JsonArrayModelBinderAttribute : ModelBinderAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="JsonArrayModelBinderAttribute"/> class.
	/// </summary>
	public JsonArrayModelBinderAttribute()
	{
		BinderType = typeof(JsonArrayModelBinder);
	}
}