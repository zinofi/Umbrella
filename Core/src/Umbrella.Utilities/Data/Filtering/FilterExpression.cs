﻿using System.Linq.Expressions;
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
public readonly record struct FilterExpression<TItem> : IDataExpression<TItem>
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
}