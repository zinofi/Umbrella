// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Linq.Expressions;
using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Helpers;

namespace Umbrella.Utilities.Expressions;

/// <summary>
/// Helps building dynamic expressions.
/// </summary>
public static class UmbrellaDynamicExpression
{
	private static readonly ConcurrentDictionary<Type, Func<string, IFormatProvider?, object>> _cache = new();

	/// <summary>
	/// Create a dynamic comparison expression for a given property selector, comparison method and reference value.
	/// </summary>
	/// <param name="target">The parameter of the query data.</param>
	/// <param name="selector">The property selector to parse.</param>
	/// <param name="comparer">The comparison method to use.</param>
	/// <param name="value">The reference value to compare with.</param>
	/// <param name="provider">The culture-specific formatting information.</param>
	/// <returns>The dynamic comparison expression.</returns>
	public static Expression CreateComparison(ParameterExpression target, string selector, UmbrellaDynamicCompare comparer, string value, IFormatProvider? provider = null)
	{
		Guard.IsNotNull(target, nameof(target));
		Guard.IsNotNullOrWhiteSpace(selector, nameof(selector));

		if (!Enum.IsDefined(typeof(UmbrellaDynamicCompare), comparer))
			throw new ArgumentOutOfRangeException(nameof(comparer));

		var memberAccess = CreateMemberAccess(target, selector) ?? throw new InvalidOperationException("The memberAccess is null.");
		var actualValue = CreateConstant(target, memberAccess, value, provider);

		return comparer switch
		{
			UmbrellaDynamicCompare.Equal => Expression.Equal(memberAccess, actualValue),
			UmbrellaDynamicCompare.NotEqual => Expression.NotEqual(memberAccess, actualValue),
			UmbrellaDynamicCompare.GreaterThan => Expression.GreaterThan(memberAccess, actualValue),
			UmbrellaDynamicCompare.GreaterThanOrEqual => Expression.GreaterThanOrEqual(memberAccess, actualValue),
			UmbrellaDynamicCompare.LessThan => Expression.LessThan(memberAccess, actualValue),
			UmbrellaDynamicCompare.LessThanOrEqual => Expression.LessThanOrEqual(memberAccess, actualValue),
			_ => Expression.Constant(false),
		};
	}

	/// <summary>
	/// Create a dynamic comparison expression for a given property selector, comparison method and reference value.
	/// </summary>
	/// <param name="target">The parameter of the query data.</param>
	/// <param name="selector">The property selector to parse.</param>
	/// <param name="comparer">The comparison method to use.</param>
	/// <param name="value">The reference value to compare with.</param>
	/// <param name="provider">The culture-specific formatting information.</param>
	/// <returns>The dynamic comparison expression.</returns>
	public static Expression CreateComparison(ParameterExpression target, string selector, string comparer, string value, IFormatProvider? provider = null)
	{
		Guard.IsNotNull(target, nameof(target));
		Guard.IsNotNullOrWhiteSpace(selector, nameof(selector));
		Guard.IsNotNullOrWhiteSpace(comparer, nameof(comparer));

		var memberAccess = CreateMemberAccess(target, selector) ?? throw new InvalidOperationException("The memberAccess is null.");
		var actualValue = CreateConstant(target, memberAccess, value, provider);

		return Expression.Call(memberAccess, comparer, null, actualValue);
	}

	/// <summary>
	/// Creates a dynamic member access expression.
	/// </summary>
	/// <param name="target">The parameter of the query data.</param>
	/// <param name="selector">The property selector to parse.</param>
	/// <param name="throwOnError">Specifies if an exception is thrown if an error is encountered. If not, this will just return <see langword="null"/>.</param>
	/// <returns>The dynamic member access expression.</returns>
	public static MemberExpression? CreateMemberAccess(ParameterExpression target, string selector, bool throwOnError = true)
	{
		Guard.IsNotNull(target, nameof(target));
		Guard.IsNotNullOrWhiteSpace(selector, nameof(selector));

		try
		{
			return selector.Split('.').Aggregate((Expression)target, Expression.PropertyOrField) as MemberExpression;
		}
		catch (Exception exc)
		{
			return throwOnError
				? throw new UmbrellaException($"An error occurred whilst trying to create the member access for '{target.Name}' using selector '{selector}'.", exc)
				: null;
		}
	}

	internal static Expression CreateConstant(ParameterExpression target, Expression selector, string value, IFormatProvider? provider)
	{
		var type = Expression.Lambda(selector, target).ReturnType;
		var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

		if (string.IsNullOrEmpty(value))
			return Expression.Default(type);

		if (!underlyingType.IsEnum || !EnumHelper.TryParseEnum(underlyingType, value, true, out object? convertedValue))
		{
			var converter = _cache.GetOrAdd(type, CreateConverter);
			convertedValue = converter(value, provider);
		}

		return Expression.Constant(convertedValue, type);
	}

	private static Func<string, IFormatProvider?, object> CreateConverter(Type type)
	{
		var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

		var target = Expression.Parameter(typeof(string));
		var format = Expression.Parameter(typeof(IFormatProvider));

		var expression = (Expression)target;

		var ordinalParse = underlyingType.GetMethod("Parse", new[] { typeof(string) });

		if (ordinalParse is not null)
			expression = Expression.Call(ordinalParse, target);

		var cultureParse = underlyingType.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });

		if (cultureParse is not null)
			expression = Expression.Call(cultureParse, target, format);

		return Expression.Lambda<Func<string, IFormatProvider?, object>>(
			Expression.Convert(expression, typeof(object)), target, format).Compile();
	}
}