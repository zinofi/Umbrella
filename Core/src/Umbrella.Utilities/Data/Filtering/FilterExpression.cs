// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Data.Filtering;

/// <summary>
/// Specifies how items of <typeparamref name="TItem"/> should be filtered.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly struct FilterExpression<TItem> : IEquatable<FilterExpression<TItem>>, IDataExpression<TItem>
{
	private readonly Lazy<Func<TItem, object>>? _lazyFunc;
	private readonly Lazy<string>? _lazyMemberPath;

	/// <inheritdoc />
	public Expression<Func<TItem, object>>? Expression { get; }

	/// <inheritdoc />
	public string? MemberPath => _lazyMemberPath?.Value;

	/// <summary>
	/// Gets the value used for filtering.
	/// </summary>
	public object? Value { get; }

	/// <summary>
	/// Gets the filter type.
	/// </summary>
	public FilterType Type { get; }

	/// <summary>
	/// Gets or sets whether or not this filter is a primary filter.
	/// </summary>
	public bool IsPrimary { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="FilterExpression{TItem}"/> struct.
	/// </summary>
	/// <param name="expression">The expression.</param>
	/// <param name="value">The value.</param>
	/// <param name="type">The type.</param>
	/// <param name="isPrimary">Specifies whether this is a primary filter.</param>
	public FilterExpression(Expression<Func<TItem, object>> expression, object? value, FilterType type = FilterType.Contains, bool isPrimary = false)
	{
		Guard.IsNotNull(expression, nameof(expression));

		Expression = expression;
		Value = value;
		Type = type;
		IsPrimary = isPrimary;

		_lazyFunc = new Lazy<Func<TItem, object>>(expression.Compile);
		_lazyMemberPath = new Lazy<string>(expression.GetMemberPath);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="FilterExpression{TItem}"/> struct.
	/// </summary>
	/// <param name="expression">The expression.</param>
	/// <param name="delegate">The delegate.</param>
	/// <param name="memberPath">The member path.</param>
	/// <param name="value">The value.</param>
	/// <param name="type">The type.</param>
	/// <param name="isPrimary">Specifies whether this is a primary filter.</param>
	/// <remarks>This is dynamically invoked by the <see cref="DataExpressionFactory"/>.</remarks>
	public FilterExpression(LambdaExpression expression, Lazy<Delegate> @delegate, Lazy<string> memberPath, object? value, FilterType type, bool isPrimary)
	{
		Expression = (Expression<Func<TItem, object>>)expression;
		Value = value;
		Type = type;
		IsPrimary = isPrimary;

		_lazyFunc = new Lazy<Func<TItem, object>>(() => (Func<TItem, object>)@delegate.Value);
		_lazyMemberPath = memberPath;
	}

	/// <inheritdoc />
	public Func<TItem, object>? GetDelegate() => _lazyFunc?.Value;

	/// <inheritdoc />
	public override string ToString() => $"{MemberPath} - {Value} - {Type} - {IsPrimary}";

	/// <summary>
	/// Converts to this instance to a <see cref="FilterExpressionDescriptor"/>.
	/// </summary>
	/// <returns>The <see cref="FilterExpressionDescriptor"/> instance.</returns>
	public FilterExpressionDescriptor? ToFilterExpressionDescriptor()
		=> (FilterExpressionDescriptor?)this;

	/// <inheritdoc />
	public override bool Equals(object obj) => obj is FilterExpression<TItem> expression && Equals(expression);

	/// <inheritdoc />
	public bool Equals(FilterExpression<TItem> other) => EqualityComparer<Func<TItem, object>?>.Default.Equals(GetDelegate(), other.GetDelegate()) &&
			   EqualityComparer<Expression<Func<TItem, object>>?>.Default.Equals(Expression, other.Expression) &&
			   MemberPath == other.MemberPath &&
			   EqualityComparer<object?>.Default.Equals(Value, other.Value) &&
			   Type == other.Type;

	/// <inheritdoc />
	public override int GetHashCode()
	{
		int hashCode = -1510804887;
		hashCode = (hashCode * -1521134295) + EqualityComparer<Func<TItem, object>?>.Default.GetHashCode(GetDelegate());
		hashCode = (hashCode * -1521134295) + EqualityComparer<Expression<Func<TItem, object>>?>.Default.GetHashCode(Expression);
		hashCode = (hashCode * -1521134295) + EqualityComparer<string?>.Default.GetHashCode(MemberPath);
		hashCode = (hashCode * -1521134295) + EqualityComparer<object?>.Default.GetHashCode(Value);
		hashCode = (hashCode * -1521134295) + Type.GetHashCode();
		return hashCode;
	}

	/// <summary>
	/// Performs an explicit conversion from <see cref="FilterExpression{TItem}"/> to <see cref="FilterExpressionDescriptor"/>.
	/// </summary>
	/// <param name="filterExpression">The filter expression.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static explicit operator FilterExpressionDescriptor?(FilterExpression<TItem> filterExpression)
		=> filterExpression != default && filterExpression.MemberPath is not null
			? new FilterExpressionDescriptor(filterExpression.MemberPath, filterExpression.Value!.ToString(), filterExpression.Type, filterExpression.IsPrimary)
			: null;

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