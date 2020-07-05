using System;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression
{
	/// <summary>
	/// A helper for use with Data Expression model binders.
	/// </summary>
	public static class DataExpressionModelBinderHelper
	{
		/// <summary>
		/// The generic sort expression type
		/// </summary>
		public static readonly Type SortExpressionType = typeof(SortExpression<>);

		/// <summary>
		/// The generic filter expression type
		/// </summary>
		public static readonly Type FilterExpressionType = typeof(FilterExpression<>);
	}
}