using System;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor.Sorting;

/// <summary>
/// A model binder that binds an incoming JSON string to either an array or a single instance of <see cref="SortExpressionDescriptor"/>.
/// </summary>
public class SortExpressionDescriptorModelBinder : DataExpressionDescriptorModelBinder<SortExpressionDescriptor>
{
	/// <inheritdoc />
	protected override Type DescriptorType => DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.DescriptorType;

	/// <inheritdoc />
	protected override Type EnumerableDescriptorType => DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.EnumerableDescriptorType;

	/// <summary>
	/// Initializes a new instance of the <see cref="SortExpressionDescriptorModelBinder"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dataExpressionFactory">The data expression factory.</param>
	public SortExpressionDescriptorModelBinder(
		ILogger<SortExpressionDescriptorModelBinder> logger,
		IDataExpressionFactory dataExpressionFactory)
		: base(logger, dataExpressionFactory)
	{
	}
}