using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An equality comparer for comparing instances of <see cref="IDataExpressionDescriptor"/> by comparing the <see cref="IDataExpressionDescriptor.MemberPath"/>
	/// property using ordinal case-insensitive rules.
	/// </summary>
	public class DataExpressionDescriptorComparer : DataExpressionDescriptorComparer<IDataExpressionDescriptor>
	{
	}

	/// <summary>
	/// An equality comparer for comparing instances of <typeparamref name="TDescriptor"/> by comparing the <see cref="IDataExpressionDescriptor.MemberPath"/>
	/// property using ordinal case-insensitive rules.
	/// </summary>
	/// <typeparam name="TDescriptor">The implementation of <see cref="IDataExpressionDescriptor" /></typeparam>
	public class DataExpressionDescriptorComparer<TDescriptor> : EqualityComparer<TDescriptor>
		where TDescriptor : IDataExpressionDescriptor
	{
		/// <inheritdoc />
		public override bool Equals(TDescriptor x, TDescriptor y)
		{
			if (x?.MemberPath is null && y?.MemberPath is null)
				return true;

			if (x?.MemberPath is null)
				return false;

			if (y?.MemberPath is null)
				return false;

			return x.MemberPath.Trim().Equals(y.MemberPath.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		/// <inheritdoc />
		public override int GetHashCode(TDescriptor obj) => obj?.MemberPath?.ToUpperInvariant()?.GetHashCode() ?? -1;
	}
}