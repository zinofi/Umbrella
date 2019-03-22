using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Sorting
{
	//TODO: Implement some kind of caching in place of the lazies.
	//This is potentially dangerous though due to the difficulty of determing
	//equality between 2 expressions and therefore uniqueness which is required
	//for caching. The cost of computing some kind of unique key might then be more
	//expensive than just sticking with the Lazy solution we already have.
	public readonly struct SortExpression<TItem>
	{
		private readonly Lazy<Func<TItem, object>> _lazyFunc;
		private readonly Lazy<string> _lazyMemberName;

		public SortDirection Direction { get; }

		public Func<TItem, object> Func => _lazyFunc.Value;

		public Expression<Func<TItem, object>> Expression { get; }

		public string MemberName => _lazyMemberName.Value;

		public SortExpression(Expression<Func<TItem, object>> expression, SortDirection direction)
		{
			Guard.ArgumentNotNull(expression, nameof(expression));

			Direction = direction;
			Expression = expression;

			_lazyFunc = new Lazy<Func<TItem, object>>(() => expression.Compile());
			_lazyMemberName = new Lazy<string>(() => expression.GetMemberName());
		}

		public SortExpressionSerializable ToSortExpressionSerializable()
			=> (SortExpressionSerializable)this;

		public static explicit operator SortExpressionSerializable(SortExpression<TItem> sortExpression)
			=> new SortExpressionSerializable(sortExpression.MemberName, sortExpression.Direction.ToString());
	}
}