using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression;

/// <summary>
/// A delegate used to apply one or more transformations to one or more <see cref="IDataExpressionDescriptor"/> instances.
/// </summary>
/// <typeparam name="TDescriptor">The type of the descriptor.</typeparam>
/// <param name="context">The context.</param>
/// <param name="descriptors">The descriptors.</param>
/// <param name="length">The length.</param>
public delegate void DataExpressionTransformer<TDescriptor>(ModelBindingContext context, IEnumerable<TDescriptor> descriptors, int length)
	where TDescriptor : IDataExpressionDescriptor;