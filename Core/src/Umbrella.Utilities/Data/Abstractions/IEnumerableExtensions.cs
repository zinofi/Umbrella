// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/> collections for data expressions.
	/// </summary>
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Finds the <see cref="SortExpressionDescriptor" /> with the specified <paramref name="memberPath"/> and applies it to the specified <paramref name="sorters"/>
		/// using the provided <paramref name="expression"/> as a new <see cref="SortExpression{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of the item being sorted.</typeparam>
		/// <param name="descriptors">The descriptors to search.</param>
		/// <param name="sorters">The existing sorters to which the new sorter will be added.</param>
		/// <param name="memberPath">The member path.</param>
		/// <param name="expression">The expression used to sort the item.</param>
		public static void FindAndApplySortExpressionDescriptor<T>(this IEnumerable<IDataExpressionDescriptor> descriptors, ref SortExpression<T>[]? sorters, string memberPath, Expression<Func<T, object>> expression)
		{
			SortExpressionDescriptor? sortExpressionDescriptor = descriptors.FindSortExpressionDescriptor(memberPath);

			if (sortExpressionDescriptor is null)
				return;

			var lstSorter = sorters?.ToList() ?? new List<SortExpression<T>>();
			lstSorter.Add(new SortExpression<T>(expression, sortExpressionDescriptor.Direction));
			sorters = lstSorter.ToArray();
		}
	}
}