using System;
using System.Collections.Generic;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpressionDescriptor;

/// <summary>
/// A helper for use with Data Expression model binders and their descriptor counterparts.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class DataExpressionDescriptorModelBinderHelper<T>
{
	/// <summary>
	/// The descriptor type
	/// </summary>
	public static readonly Type DescriptorType = typeof(T);

	/// <summary>
	/// The enumerable descriptor type
	/// </summary>
	public static readonly Type EnumerableDescriptorType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
}