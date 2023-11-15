using System.Web.Http.ModelBinding;

namespace Umbrella.Legacy.WebUtilities.WebApi.ModelBinding;

/// <summary>
/// Specifies that the target action method parameter uses the <see cref="ArrayWrapperModelBinder"/>.
/// </summary>
/// <seealso cref="ModelBinderAttribute" />
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ArrayWrapperModelBinderAttribute : ModelBinderAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ArrayWrapperModelBinderAttribute"/> class.
	/// </summary>
	public ArrayWrapperModelBinderAttribute()
		: base(typeof(ArrayWrapperModelBinder))
	{
	}
}