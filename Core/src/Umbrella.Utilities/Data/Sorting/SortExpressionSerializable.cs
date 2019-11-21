using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// A serializable version of the <see cref="SortExpression{TItem}"/> type.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public readonly struct SortExpressionSerializable : IEquatable<SortExpressionSerializable>
	{
		internal SortExpressionSerializable(string memberName, string direction)
		{
			MemberName = memberName;
			Direction = direction;
		}

		/// <summary>
		/// Gets the name of the member.
		/// </summary>
		public string MemberName { get; }

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public string Direction { get; }

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) => obj is SortExpressionSerializable serializable && Equals(serializable);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(SortExpressionSerializable other) => MemberName == other.MemberName &&
				   Direction == other.Direction;

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			int hashCode = 1674265682;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Direction);
			return hashCode;
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(SortExpressionSerializable left, SortExpressionSerializable right) => left.Equals(right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(SortExpressionSerializable left, SortExpressionSerializable right) => !(left == right);
	}
}