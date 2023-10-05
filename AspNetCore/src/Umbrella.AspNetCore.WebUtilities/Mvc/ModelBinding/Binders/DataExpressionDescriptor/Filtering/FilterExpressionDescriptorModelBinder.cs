using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Filtering;

/// <summary>
/// A model binder that binds an incoming JSON string to either an array or a single instance of <see cref="FilterExpressionDescriptor"/>.
/// </summary>
public class FilterExpressionDescriptorModelBinder : DataExpressionDescriptorModelBinder<FilterExpressionDescriptor>
{
	/// <inheritdoc />
	protected override Type DescriptorType => DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.DescriptorType;

	/// <inheritdoc />
	protected override Type EnumerableDescriptorType => DataExpressionDescriptorModelBinderHelper<FilterExpressionDescriptor>.EnumerableDescriptorType;

	/// <summary>
	/// Initializes a new instance of the <see cref="FilterExpressionDescriptorModelBinder"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dataExpressionFactory">The data expression factory.</param>
	public FilterExpressionDescriptorModelBinder(
		ILogger<FilterExpressionDescriptorModelBinder> logger,
		IDataExpressionFactory dataExpressionFactory)
		: base(logger, dataExpressionFactory)
	{
	}
}