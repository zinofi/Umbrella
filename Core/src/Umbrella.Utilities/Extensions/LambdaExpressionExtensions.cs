using System;
using System.Linq.Expressions;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="LambdaExpression"/> type.
	/// </summary>
	public static class LambdaExpressionExtensions
	{
		/// <summary>
		/// Transforms the specified expression into a path string,
		/// e.g. it will transform Parent -> Child -> Name into Parent.Child.Name.
		/// This is useful for use with EF Core.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The path string.</returns>
		public static string AsPath(this LambdaExpression expression)
		{
			if (expression == null)
				return null;

			var exp = expression.Body;
			TryParsePath(exp, out string path);

			return path;
		}

		// This method is a slight modification of EF6 source code
		private static bool TryParsePath(Expression expression, out string path)
		{
			path = null;

			Expression withoutConvert = expression;

			while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			if (withoutConvert is MemberExpression memberExpression)
			{
				string thisPart = memberExpression.Member.Name;

				if (!TryParsePath(memberExpression.Expression, out string parentPart))
					return false;

				path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
			}
			else if (withoutConvert is MethodCallExpression callExpression)
			{
				if (callExpression.Method.Name == "Select" && callExpression.Arguments.Count == 2)
				{
					if (!TryParsePath(callExpression.Arguments[0], out string parentPart))
						return false;

					if (parentPart != null)
					{
						if (callExpression.Arguments[1] is LambdaExpression subExpression)
						{
							if (!TryParsePath(subExpression.Body, out string thisPart))
								return false;

							if (thisPart != null)
							{
								path = parentPart + "." + thisPart;
								return true;
							}
						}
					}
				}
				else if (callExpression.Method.Name == "Where")
				{
					throw new NotSupportedException("Filtering an Include expression is not supported");
				}
				else if (callExpression.Method.Name == "OrderBy" || callExpression.Method.Name == "OrderByDescending")
				{
					throw new NotSupportedException("Ordering an Include expression is not supported");
				}

				return false;
			}

			return true;
		}
	}
}