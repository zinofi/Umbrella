using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Umbrella.Utilities.Data.Filtering;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Filtering;

/// <summary>
/// A model binder provider for the <see cref="FilterExpressionDescriptorModelBinder"/>.
/// </summary>
/// <seealso cref="IModelBinderProvider" />
public class FilterExpressionDescriptorModelBinderProvider : IModelBinderProvider
{
	/// <inheritdoc />
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		Guard.IsNotNull(context);

		if (context.Metadata.UnderlyingOrModelType == DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.DescriptorType || DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.EnumerableDescriptorType.IsAssignableFrom(context.Metadata.ModelType))
			return new BinderTypeModelBinder(typeof(FilterExpressionDescriptorModelBinder));

		return null;
	}
}