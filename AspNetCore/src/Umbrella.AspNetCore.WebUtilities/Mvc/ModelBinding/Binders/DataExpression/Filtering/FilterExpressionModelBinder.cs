using System;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Filtering;

/// <summary>
/// A model binder that binds an incoming JSON string to either an array or a single instance of a <see cref="FilterExpression{TItem}"/>.
/// </summary>
public class FilterExpressionModelBinder : DataExpressionModelBinder<FilterExpressionDescriptor>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="FilterExpressionModelBinder"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dataExpressionFactory">The data expression factory.</param>
	public FilterExpressionModelBinder(
		ILogger<FilterExpressionModelBinder> logger,
		IDataExpressionFactory dataExpressionFactory)
		: base(logger, dataExpressionFactory)
	{
	}

	/// <inheritdoc />
	protected override Type DescriptorType => DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.DescriptorType;

	/// <inheritdoc />
	protected override Type EnumerableDescriptorType => DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.EnumerableDescriptorType;
}