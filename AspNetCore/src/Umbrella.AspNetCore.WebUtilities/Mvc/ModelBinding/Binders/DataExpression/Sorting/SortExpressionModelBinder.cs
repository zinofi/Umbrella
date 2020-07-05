using System;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Sorting
{
	/// <summary>
	/// A model binder that binds an incoming JSON string to either an array or a single instance of a <see cref="SortExpression{TItem}"/>.
	/// </summary>
	public class SortExpressionModelBinder : DataExpressionModelBinder<SortExpressionDescriptor>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SortExpressionModelBinder"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataExpressionFactory">The data expression factory.</param>
		public SortExpressionModelBinder(
			ILogger<SortExpressionModelBinder> logger,
			IDataExpressionFactory dataExpressionFactory)
			: base(logger, dataExpressionFactory)
		{
		}

		/// <inheritdoc />
		protected override Type DescriptorType => DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.DescriptorType;

		/// <inheritdoc />
		protected override Type EnumerableDescriptorType => DataExpressionDescriptorModelBinderHelper<SortExpressionDescriptor>.EnumerableDescriptorType;
	}
}