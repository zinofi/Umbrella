using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Filtering;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Sorting;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Filtering;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Sorting;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders;

/// <summary>
/// Extension methods for the <see cref="MvcOptions"/> class.
/// </summary>
public static class MvcOptionsExtensions
{
	/// <summary>
	/// Inserts the Umbrella model binders into the <see cref="MvcOptions.ModelBinderProviders"/> collection
	/// at the specified <paramref name="startIndex"/>.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <param name="startIndex">The start index where the model binders will be inserted in the <see cref="MvcOptions.ModelBinderProviders"/> collection.</param>
	/// <param name="sortExpressionTransformer">The sort expression transformer.</param>
	/// <param name="filterExpressionTransformer">The filter expression transformer.</param>
	/// <returns>The final insertion index.</returns>
	public static int InsertUmbrellaModelBinders(
		this MvcOptions options,
		int startIndex = 0,
		DataExpressionTransformer<SortExpressionDescriptor>? sortExpressionTransformer = null,
		DataExpressionTransformer<FilterExpressionDescriptor>? filterExpressionTransformer = null)
	{
		Guard.IsNotNull(options);

		options.ModelBinderProviders.Insert(startIndex, new SortExpressionDescriptorModelBinderProvider());
		options.ModelBinderProviders.Insert(++startIndex, new SortExpressionModelBinderProvider());
		options.ModelBinderProviders.Insert(++startIndex, new FilterExpressionDescriptorModelBinderProvider());
		options.ModelBinderProviders.Insert(++startIndex, new FilterExpressionModelBinderProvider());

		SortExpressionModelBinder.DescriptorTransformer = sortExpressionTransformer;
		FilterExpressionModelBinder.DescriptorTransformer = filterExpressionTransformer;

		return startIndex;
	}
}