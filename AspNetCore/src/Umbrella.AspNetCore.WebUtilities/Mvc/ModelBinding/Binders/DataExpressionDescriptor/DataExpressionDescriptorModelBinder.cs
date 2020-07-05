using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor
{
	/// <summary>
	/// Serves as the base class for all Data Expression Descriptor model binders.
	/// </summary>
	/// <typeparam name="TDescriptor">The type of the descriptor.</typeparam>
	public abstract class DataExpressionDescriptorModelBinder<TDescriptor> : DataExpressionModelBinder<TDescriptor>
		where TDescriptor : IDataExpressionDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataExpressionDescriptorModelBinder{TDescriptor}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataExpressionFactory">The data expression factory.</param>
		public DataExpressionDescriptorModelBinder(
			ILogger logger,
			IDataExpressionFactory dataExpressionFactory)
			: base(logger, dataExpressionFactory)
		{
		}

		/// <inheritdoc />
		protected override object? TransformDescriptor(Type underlyingOrModelType, TDescriptor descriptor) => descriptor;

		/// <inheritdoc />
		protected override object? TransformDescriptors(Type underlyingOrModelType, IEnumerable<TDescriptor> descriptors) => descriptors switch
		{
			TDescriptor[] _ => descriptors.Cast<TDescriptor>().ToArray(),
			_ => descriptors.Cast<TDescriptor>().ToList()
		};
	}
}