using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Data.Sorting
{
	[StructLayout(LayoutKind.Auto)]
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

		public override string ToString() => $"{MemberName} - {Direction}";

		public SortExpressionSerializable ToSortExpressionSerializable()
			=> (SortExpressionSerializable)this;

		public static explicit operator SortExpressionSerializable(SortExpression<TItem> sortExpression)
			=> new SortExpressionSerializable(sortExpression.MemberName, sortExpression.Direction.ToString());
	}
}