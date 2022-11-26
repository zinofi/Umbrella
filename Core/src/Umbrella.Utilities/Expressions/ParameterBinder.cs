// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Linq.Expressions;
using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.Expressions;

/// <summary>
/// Rebinds a parameter expression to any expression.
/// </summary>
public class ParameterBinder : ExpressionVisitor
{
	private readonly ParameterExpression _parameter;
	private readonly Expression _replacement;

	/// <summary>
	/// Create an new binder.
	/// </summary>
	/// <param name="parameter">Parameter to find.</param>
	/// <param name="replacement">Expression to insert.</param>
	public ParameterBinder(ParameterExpression parameter, Expression replacement)
	{
		Guard.IsNotNull(parameter);
		Guard.IsNotNull(replacement);

		_parameter = parameter;
		_replacement = replacement;
	}

	/// <inheritdoc />
	protected override Expression VisitParameter(ParameterExpression node)
	{
		Guard.IsNotNull(node);

		return node == _parameter
			? _replacement
			: base.VisitParameter(node);
	}

	/// <inheritdoc />
	protected override Expression VisitInvocation(InvocationExpression node)
	{
		Guard.IsNotNull(node);

		if (node.Expression == _parameter && _replacement is LambdaExpression lambda)
		{
			var binders = lambda.Parameters.Zip(node.Arguments,
				(p, a) => new ParameterBinder(p, a));

			return binders.Aggregate(lambda.Body, (e, b) => b.Visit(e));
		}

		return base.VisitInvocation(node);
	}
}