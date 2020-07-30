using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Data.Filtering
{
	// TOOD: Scrap these in favour of just having the descriptor and mark as [Serializable]??

	/// <summary>
	/// A serializable version of the <see cref="FilterExpression{TItem}"/> type.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public readonly struct FilterExpressionSerializable : IEquatable<FilterExpressionSerializable>
	{
		internal FilterExpressionSerializable(string memberPath, string value, string type)
		{
			MemberPath = memberPath;
			Value = value;
			Type = type;
		}

		/// <summary>
		/// Gets the path of the member.
		/// </summary>
		public string MemberPath { get; }

		/// <summary>
		/// Gets the filter value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Gets the type of the filter.
		/// </summary>
		public string Type { get; }

		/// <inheritdoc />
		public override bool Equals(object obj) => obj is FilterExpressionSerializable serializable && Equals(serializable);

		/// <inheritdoc />
		public bool Equals(FilterExpressionSerializable other) => MemberPath == other.MemberPath &&
				   Value == other.Value &&
				   Type == other.Type;

		/// <inheritdoc />
		public override int GetHashCode()
		{
			int hashCode = -478253073;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberPath);
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
		public static bool operator ==(FilterExpressionSerializable left, FilterExpressionSerializable right) => left.Equals(right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(FilterExpressionSerializable left, FilterExpressionSerializable right) => !(left == right);
	}
}