using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Specifies how items of <typeparamref name="TItem"/> should be filtered.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	[StructLayout(LayoutKind.Auto)]
	public readonly struct FilterExpression<TItem> : IEquatable<FilterExpression<TItem>>
	{
		private readonly Lazy<Func<TItem, object>> _lazyFunc;
		private readonly Lazy<string> _lazyMemberName;

		/// <summary>
		/// Gets the compiled <see cref="Expression"/>.
		/// </summary>
		public Func<TItem, object> Func => _lazyFunc.Value;

		/// <summary>
		/// Gets the expression.
		/// </summary>
		public Expression<Func<TItem, object>> Expression { get; }

		/// <summary>
		/// Gets the name of the member.
		/// </summary>
		public string MemberName => _lazyMemberName.Value;

		/// <summary>
		/// Gets the value used for filtering.
		/// </summary>
		public object Value { get; }

		/// <summary>
		/// Gets the filter type.
		/// </summary>
		public FilterType Type { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterExpression{TItem}"/> struct.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public FilterExpression(Expression<Func<TItem, object>> expression, object value, FilterType type = FilterType.Contains)
		{
			Guard.ArgumentNotNull(expression, nameof(expression));

			Expression = expression;
			Value = value;
			Type = type;

			_lazyFunc = new Lazy<Func<TItem, object>>(() => expression.Compile());
			_lazyMemberName = new Lazy<string>(() => expression.GetMemberName());
		}

		/// <summary>
		/// Converts this instance to a string.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString() => $"{MemberName} - {Value.ToString()} - {Type}";

		/// <summary>
		/// Converts to this instance to a <see cref="FilterExpressionSerializable"/>.
		/// </summary>
		/// <returns>The <see cref="FilterExpressionSerializable"/> instance.</returns>
		public FilterExpressionSerializable ToFilterExpressionSerializable()
			=> (FilterExpressionSerializable)this;

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) => obj is FilterExpression<TItem> expression && Equals(expression);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(FilterExpression<TItem> other) => EqualityComparer<Func<TItem, object>>.Default.Equals(Func, other.Func) &&
				   EqualityComparer<Expression<Func<TItem, object>>>.Default.Equals(Expression, other.Expression) &&
				   MemberName == other.MemberName &&
				   EqualityComparer<object>.Default.Equals(Value, other.Value) &&
				   Type == other.Type;

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			int hashCode = -1510804887;
			hashCode = hashCode * -1521134295 + EqualityComparer<Func<TItem, object>>.Default.GetHashCode(Func);
			hashCode = hashCode * -1521134295 + EqualityComparer<Expression<Func<TItem, object>>>.Default.GetHashCode(Expression);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberName);
			hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
			hashCode = hashCode * -1521134295 + Type.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="FilterExpression{TItem}"/> to <see cref="FilterExpressionSerializable"/>.
		/// </summary>
		/// <param name="filterExpression">The filter expression.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static explicit operator FilterExpressionSerializable(FilterExpression<TItem> filterExpression)
			=> new FilterExpressionSerializable(filterExpression.MemberName, filterExpression.Value.ToString(), filterExpression.Type.ToString());

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(FilterExpression<TItem> left, FilterExpression<TItem> right) => left.Equals(right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(FilterExpression<TItem> left, FilterExpression<TItem> right) => !(left == right);
	}
}