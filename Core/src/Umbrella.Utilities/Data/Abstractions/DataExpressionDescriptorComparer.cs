using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An equality comparer for comparing instances of <see cref="IDataExpressionDescriptor"/> by comparing the <see cref="IDataExpressionDescriptor.MemberName"/>
	/// property using ordinal case-insensitive rules.
	/// </summary>
	public class DataExpressionDescriptorComparer : DataExpressionDescriptorComparer<IDataExpressionDescriptor>
	{
	}

	/// <summary>
	/// An equality comparer for comparing instances of <typeparamref name="TDescriptor"/> by comparing the <see cref="IDataExpressionDescriptor.MemberName"/>
	/// property using ordinal case-insensitive rules.
	/// </summary>
	/// <typeparam name="TDescriptor">The implementation of <see cref="IDataExpressionDescriptor" /></typeparam>
	public class DataExpressionDescriptorComparer<TDescriptor> : EqualityComparer<TDescriptor>
		where TDescriptor : IDataExpressionDescriptor
	{
		/// <summary>
		/// Determines whether two objects of type <typeparamref name="TDescriptor"/> are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(TDescriptor x, TDescriptor y)
		{
			if (x?.MemberName is null && y?.MemberName is null)
				return true;

			if (x?.MemberName is null)
				return false;

			if (y?.MemberName is null)
				return false;

			return x.MemberName.Trim().Equals(y.MemberName.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode(TDescriptor obj) => obj?.MemberName?.ToUpperInvariant()?.GetHashCode() ?? -1;
	}
}