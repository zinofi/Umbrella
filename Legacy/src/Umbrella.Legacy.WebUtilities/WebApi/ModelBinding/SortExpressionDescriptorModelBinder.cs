using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Newtonsoft.Json;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.Legacy.WebUtilities.WebApi.ModelBinding
{
	public class SortExpressionDescriptorModelBinder : IModelBinder
	{
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			// TODO: Check this can actually deal with an action method that doesn't take an array
			if (bindingContext.ModelType == typeof(SortExpressionDescriptor) || typeof(IEnumerable<>).MakeGenericType(typeof(SortExpressionDescriptor)).IsAssignableFrom(bindingContext.ModelType))
			{
				ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

				if (val == null)
				{
					return false;
				}

				if (!(val.RawValue is string rawValue))
				{
					bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Wrong value type for {nameof(SortExpressionDescriptor)}");
					return false;
				}

				try
				{
					bindingContext.Model = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType);

					return true;
				}
				catch
				{
					bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Cannot convert value to {nameof(SortExpressionDescriptor)}.");
				}
			}

			return false;
		}
	}
}