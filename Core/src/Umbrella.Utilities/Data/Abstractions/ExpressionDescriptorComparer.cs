using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An equality comparer for comparing instances of <see cref="IExpressionDescriptor"/> by comparing the <see cref="IExpressionDescriptor.MemberName"/>
	/// property using ordinal case-insensitive rules.
	/// </summary>
	/// <seealso cref="T:System.Collections.Generic.EqualityComparer{Umbrella.Utilities.Data.Abstractions.IExpressionDescriptor}" />
	public class ExpressionDescriptorComparer : EqualityComparer<IExpressionDescriptor>
	{
		/// <summary>
		/// Determines whether two objects of type <see cref="IExpressionDescriptor"/> are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(IExpressionDescriptor x, IExpressionDescriptor y)
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
		public override int GetHashCode(IExpressionDescriptor obj) => obj?.MemberName?.ToUpperInvariant()?.GetHashCode() ?? -1;
	}
}