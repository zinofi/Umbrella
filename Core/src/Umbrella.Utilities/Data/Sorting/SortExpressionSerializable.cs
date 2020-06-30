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
		internal SortExpressionSerializable(string memberPath, string direction)
		{
			MemberPath = memberPath;
			Direction = direction;
		}

		/// <summary>
		/// Gets the path of the member.
		/// </summary>
		public string MemberPath { get; }

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public string Direction { get; }

		/// <inheritdoc />
		public override bool Equals(object obj) => obj is SortExpressionSerializable serializable && Equals(serializable);

		/// <inheritdoc />
		public bool Equals(SortExpressionSerializable other) => MemberPath == other.MemberPath &&
				   Direction == other.Direction;

		/// <inheritdoc />
		public override int GetHashCode()
		{
			int hashCode = 1674265682;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberPath);
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