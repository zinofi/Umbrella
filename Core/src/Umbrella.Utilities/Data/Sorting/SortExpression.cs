using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// Specifies how items of <typeparamref name="TItem"/> should be sorted.
	/// </summary>
	/// <typeparam name="TItem">The type of the item being sorted.</typeparam>
	[StructLayout(LayoutKind.Auto)]
	public readonly struct SortExpression<TItem> : IEquatable<SortExpression<TItem>>, IDataExpression<TItem>
	{
		private readonly Lazy<Func<TItem, object>> _lazyFunc;
		private readonly Lazy<string> _lazyMemberPath;

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public SortDirection Direction { get; }

		/// <inheritdoc />
		public Expression<Func<TItem, object>> Expression { get; }

		/// <inheritdoc />
		public string MemberPath => _lazyMemberPath.Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="SortExpression{TItem}"/> struct.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="direction">The direction.</param>
		public SortExpression(Expression<Func<TItem, object>> expression, SortDirection direction)
		{
			Guard.ArgumentNotNull(expression, nameof(expression));

			Direction = direction;
			Expression = expression;

			_lazyFunc = new Lazy<Func<TItem, object>>(() => expression.Compile());
			_lazyMemberPath = new Lazy<string>(() => expression.GetMemberPath());
		}

		/// <inheritdoc />
		public Func<TItem, object> GetDelegate() => _lazyFunc.Value;

		/// <inheritdoc />
		public override string ToString() => $"{MemberPath} - {Direction}";

		/// <summary>
		/// Converts to this instance to a <see cref="SortExpressionSerializable"/> instance.
		/// </summary>
		/// <returns>The <see cref="SortExpressionSerializable"/> instance.</returns>
		public SortExpressionSerializable ToSortExpressionSerializable()
			=> (SortExpressionSerializable)this;

		/// <inheritdoc />
		public override bool Equals(object obj) => obj is SortExpression<TItem> expression && Equals(expression);

		/// <inheritdoc />
		public bool Equals(SortExpression<TItem> other) => Direction == other.Direction &&
				   EqualityComparer<Expression<Func<TItem, object>>>.Default.Equals(Expression, other.Expression) &&
				   MemberPath == other.MemberPath;

		/// <inheritdoc />
		public override int GetHashCode()
		{
			int hashCode = -155208075;
			hashCode = hashCode * -1521134295 + Direction.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Expression<Func<TItem, object>>>.Default.GetHashCode(Expression);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberPath);
			return hashCode;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="SortExpression{TItem}"/> to <see cref="SortExpressionSerializable"/>.
		/// </summary>
		/// <param name="sortExpression">The sort expression.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static explicit operator SortExpressionSerializable(SortExpression<TItem> sortExpression)
			=> new SortExpressionSerializable(sortExpression.MemberPath, sortExpression.Direction.ToString());

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(SortExpression<TItem> left, SortExpression<TItem> right) => left.Equals(right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(SortExpression<TItem> left, SortExpression<TItem> right) => !(left == right);
	}
}