using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Sorting;

/// <summary>
/// A model binder provider for the <see cref="SortExpressionDescriptorModelBinder"/>.
/// </summary>
/// <seealso cref="IModelBinderProvider" />
public class SortExpressionDescriptorModelBinderProvider : IModelBinderProvider
{
	/// <inheritdoc />
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		Guard.IsNotNull(context);

		if (context.Metadata.UnderlyingOrModelType == DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.DescriptorType || DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.EnumerableDescriptorType.IsAssignableFrom(context.Metadata.ModelType))
			return new BinderTypeModelBinder(typeof(SortExpressionDescriptorModelBinder));

		return null;
	}
}