using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Expressions;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="Expression"/> type.
	/// </summary>
	public static class ExpressionExtensions
	{
		/// <summary>
		/// Gets the name of the member specified in the supplied expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="throwException">if set to <see langword="true"/>, an exception will be thrown when the member name cannot be determined instead of returning null.</param>
		/// <returns>The member name.</returns>
		/// <exception cref="Exception">The body of the expression must be either a {nameof(MemberExpression)} or a {nameof(UnaryExpression)}.</exception>
		public static string GetMemberName(this LambdaExpression expression, bool throwException = true)
		{
			Guard.ArgumentNotNull(expression, nameof(expression));

			MemberExpression memberExpression = expression.Body switch
			{
				UnaryExpression x => x.Operand as MemberExpression,
				_ => expression.Body as MemberExpression
			};

			if (memberExpression == null && throwException)
				throw new UmbrellaException($"The body of the expression must be either a {nameof(MemberExpression)} or a {nameof(UnaryExpression)}.");

			return memberExpression?.Member?.Name;
		}

		/// <summary>
		/// Gets the full path of the member specified in the supplied expression, e.g. it will transform Parent -> Child -> Name into Parent.Child.Name.
		/// </summary>
		/// <param name="lambdaExpression">The expression.</param>
		/// <returns>The member path.</returns>
		/// <exception cref="Exception">The body of the expression must be either a {nameof(MemberExpression)} or a {nameof(UnaryExpression)}.</exception>
		public static string GetMemberPath(this LambdaExpression lambdaExpression)
		{
			Guard.ArgumentNotNull(lambdaExpression, nameof(lambdaExpression));
			
			var parts = new List<string>();

			// Inner Method
			void ParsePath(Expression expression)
			{
				while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
				{
					expression = ((UnaryExpression)expression).Operand;
				}

				if (expression is MemberExpression memberExpression)
				{
					parts.Add(memberExpression.Member.Name);
					ParsePath(memberExpression.Expression);
				}
				else if (expression is MethodCallExpression methodCallExpression && methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2)
				{
					if (methodCallExpression.Arguments[1] is LambdaExpression subExpression && subExpression.Body is MemberExpression subMemberExpression)
					{
						parts.Add(subMemberExpression.Member.Name);
						ParsePath(subMemberExpression.Expression);
					}

					if (methodCallExpression.Arguments[0] is MemberExpression innerMemberExpression)
					{
						parts.Add(innerMemberExpression.Member.Name);
						ParsePath(innerMemberExpression.Expression);
					}
				}
			}

			ParsePath(lambdaExpression.Body);

			parts.Reverse();

			return string.Join(".", parts);
		}

		/// <summary>
		/// Combines two given predicates using a conditional AND operation.
		/// </summary>
		/// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
		/// <param name="left">The first predicate expression to combine.</param>
		/// <param name="right">The second predicate expression to combine.</param>
		/// <returns>A single combined predicate expression.</returns>
		public static Expression<Func<TSource, bool>> And<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
		{
			Guard.ArgumentNotNull(left, nameof(left));
			Guard.ArgumentNotNull(right, nameof(right));

			var l = left.Parameters[0];
			var r = right.Parameters[0];

			var binder = new ParameterBinder(l, r);

			return Expression.Lambda<Func<TSource, bool>>(
				Expression.AndAlso(binder.Visit(left.Body), right.Body), r);
		}

		/// <summary>
		/// Combines two given predicates using a conditional OR operation.
		/// </summary>
		/// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
		/// <param name="left">The first predicate expression to combine.</param>
		/// <param name="right">The second predicate expression to combine.</param>
		/// <returns>A single combined predicate expression.</returns>
		public static Expression<Func<TSource, bool>> Or<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
		{
			Guard.ArgumentNotNull(left, nameof(left));
			Guard.ArgumentNotNull(right, nameof(right));

			var l = left.Parameters[0];
			var r = right.Parameters[0];

			var binder = new ParameterBinder(l, r);

			return Expression.Lambda<Func<TSource, bool>>(
				Expression.OrElse(binder.Visit(left.Body), right.Body), r);
		}
	}
}