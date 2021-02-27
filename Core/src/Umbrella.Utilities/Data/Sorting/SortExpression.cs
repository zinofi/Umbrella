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
		private readonly Lazy<Func<TItem, object>>? _lazyFunc;
		private readonly Lazy<string>? _lazyMemberPath;

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public SortDirection Direction { get; }

		/// <inheritdoc />
		public Expression<Func<TItem, object>>? Expression { get; }

		/// <inheritdoc />
		public string? MemberPath => _lazyMemberPath?.Value;

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

		/// <summary>
		/// Initializes a new instance of the <see cref="SortExpression{TItem}"/> struct.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="delegate">The delegate.</param>
		/// <param name="memberPath">The member path.</param>
		/// <param name="direction">The direction.</param>
		/// <remarks>This is dynamically invoked by the <see cref="DataExpressionFactory"/>.</remarks>
		public SortExpression(LambdaExpression expression, Lazy<Delegate> @delegate, Lazy<string> memberPath, SortDirection direction)
		{
			Direction = direction;
			Expression = (Expression<Func<TItem, object>>)expression;

			_lazyFunc = new Lazy<Func<TItem, object>>(() => (Func<TItem, object>)@delegate.Value);
			_lazyMemberPath = memberPath;
		}

		/// <inheritdoc />
		public Func<TItem, object>? GetDelegate() => _lazyFunc?.Value;

		/// <inheritdoc />
		public override string ToString() => $"{MemberPath} - {Direction}";

		/// <summary>
		/// Converts to this instance to a <see cref="SortExpressionDescriptor"/> instance.
		/// </summary>
		/// <returns>The <see cref="SortExpressionDescriptor"/> instance.</returns>
		public SortExpressionDescriptor? ToSortExpressionDescriptor()
			=> (SortExpressionDescriptor?)this;

		/// <inheritdoc />
		public override bool Equals(object obj) => obj is SortExpression<TItem> expression && Equals(expression);

		/// <inheritdoc />
		public bool Equals(SortExpression<TItem> other) => Direction == other.Direction &&
				   EqualityComparer<Expression<Func<TItem, object>>?>.Default.Equals(Expression, other.Expression) &&
				   MemberPath == other.MemberPath;

		/// <inheritdoc />
		public override int GetHashCode()
		{
			int hashCode = -155208075;
			hashCode = hashCode * -1521134295 + Direction.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Expression<Func<TItem, object>>?>.Default.GetHashCode(Expression);
			hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(MemberPath);
			return hashCode;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="SortExpression{TItem}"/> to <see cref="SortExpressionDescriptor"/>.
		/// </summary>
		/// <param name="sortExpression">The sort expression.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static explicit operator SortExpressionDescriptor?(SortExpression<TItem> sortExpression)
			=> sortExpression != default && sortExpression.MemberPath != null
				? new SortExpressionDescriptor(sortExpression.MemberPath, sortExpression.Direction)
				: null;

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