using System;
using System.Linq.Expressions;

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
		/// <typeparam name="T">The type of the object on which the member exists.</typeparam>
		/// <typeparam name="U">The type of the member on the object.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <param name="throwException">if set to <see langword="true"/>, an exception will be thrown when the member name cannot be determined instead of returning null.</param>
		/// <returns>The member name.</returns>
		/// <exception cref="Exception">The body of the expression must be either a {nameof(MemberExpression)} or a {nameof(UnaryExpression)}.</exception>
		public static string GetMemberName<T, U>(this Expression<Func<T, U>> expression, bool throwException = true)
		{
			Guard.ArgumentNotNull(expression, nameof(expression));

			MemberExpression memberExpression = null;

			if (expression.Body is MemberExpression)
				memberExpression = (MemberExpression)expression.Body;
			else if (expression.Body is UnaryExpression)
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;

			if (memberExpression == null && throwException)
				throw new Exception($"The body of the expression must be either a {nameof(MemberExpression)} or a {nameof(UnaryExpression)}.");

			return memberExpression?.Member?.Name;
		}
	}
}