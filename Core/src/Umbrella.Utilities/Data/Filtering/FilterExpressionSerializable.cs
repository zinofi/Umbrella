using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// A serializable version of the <see cref="FilterExpression{TItem, TProperty}"/> type.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public readonly struct FilterExpressionSerializable : IEquatable<FilterExpressionSerializable>
	{
		internal FilterExpressionSerializable(string memberName, string value, string type)
		{
			MemberName = memberName;
			Value = value;
			Type = type;
		}

		/// <summary>
		/// Gets the name of the member.
		/// </summary>
		public string MemberName { get; }

		/// <summary>
		/// Gets the filter value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Gets the type of the filter.
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is FilterExpressionSerializable serializable && Equals(serializable);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(FilterExpressionSerializable other)
		{
			return MemberName == other.MemberName &&
				   Value == other.Value &&
				   Type == other.Type;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			var hashCode = -478253073;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
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
		public static bool operator ==(FilterExpressionSerializable left, FilterExpressionSerializable right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(FilterExpressionSerializable left, FilterExpressionSerializable right)
		{
			return !(left == right);
		}
	}
}