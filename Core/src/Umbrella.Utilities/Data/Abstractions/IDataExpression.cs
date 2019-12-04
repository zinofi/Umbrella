using System;
using System.Linq.Expressions;

namespace Umbrella.Utilities.Data.Abstractions
{
	// TODO: Do we need these?
	public interface IDataExpression<TItem> : IDataExpression
	{
		Func<TItem, object> Func { get; }
		Expression<Func<TItem, object>> Expression { get; }
	}

	public interface IDataExpression
	{
		string MemberName { get; }
	}
}