using System;
using System.Web.Http.ModelBinding;

namespace Umbrella.Legacy.WebUtilities.WebApi.ModelBinding;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class ArrayWrapperModelBinderAttribute : ModelBinderAttribute
{
	public ArrayWrapperModelBinderAttribute()
		: base(typeof(ArrayWrapperModelBinder))
	{
	}
}