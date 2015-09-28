using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Expressions
{
	public static class SimpleExpressionHelper
	{
		public static string GetMemberName<T, U>(this Expression<Func<T, U>> expression)
		{
			MemberExpression memberExpression = (MemberExpression)null;
			if (expression.Body is MemberExpression)
				memberExpression = (MemberExpression)expression.Body;
			else if (expression.Body is UnaryExpression)
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			if (memberExpression == null)
				throw new ApplicationException("The body of the expression must be either a MemberExpression or a UnaryExpression.");

			return memberExpression.Member.Name;
		}
	}
}