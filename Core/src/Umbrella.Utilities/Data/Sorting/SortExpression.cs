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
		private readonly Lazy<string> _lazyMemberName;

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public SortDirection Direction { get; }

		/// <summary>
		/// Gets the expression.
		/// </summary>
		public Expression<Func<TItem, object>> Expression { get; }

		/// <summary>
		/// Gets the name of the member.
		/// </summary>
		public string MemberName => _lazyMemberName.Value;

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
			_lazyMemberName = new Lazy<string>(() => expression.AsPath());
		}

		/// <summary>
		/// Gets the compiled <see cref="P:Umbrella.Utilities.Data.Abstractions.IDataExpression`1.Expression" />.
		/// </summary>
		/// <returns>
		/// The compiled expression as a delegate.
		/// </returns>
		/// <remarks>
		/// This is a method rather than a property because of an issue with MVC model binding in ASP.NET Core.
		/// When reading the property value, the model validation code was throwing an exception and the only way to workaround
		/// that was to make this a method.
		/// </remarks>
		public Func<TItem, object> GetFunc() => _lazyFunc.Value;

		/// <summary>
		/// Converts this instance to a string.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString() => $"{MemberName} - {Direction}";

		/// <summary>
		/// Converts to this instance to a <see cref="SortExpressionSerializable"/> instance.
		/// </summary>
		/// <returns>The <see cref="SortExpressionSerializable"/> instance.</returns>
		public SortExpressionSerializable ToSortExpressionSerializable()
			=> (SortExpressionSerializable)this;

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) => obj is SortExpression<TItem> expression && Equals(expression);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(SortExpression<TItem> other) => Direction == other.Direction &&
				   EqualityComparer<Expression<Func<TItem, object>>>.Default.Equals(Expression, other.Expression) &&
				   MemberName == other.MemberName;

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			int hashCode = -155208075;
			hashCode = hashCode * -1521134295 + Direction.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Expression<Func<TItem, object>>>.Default.GetHashCode(Expression);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberName);
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
			=> new SortExpressionSerializable(sortExpression.MemberName, sortExpression.Direction.ToString());

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