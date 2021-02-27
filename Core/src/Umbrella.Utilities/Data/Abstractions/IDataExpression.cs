using System;
using System.Linq.Expressions;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An abstraction representing a data expression.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <seealso cref="IDataExpression" />
	public interface IDataExpression<TItem> : IDataExpression
	{
		/// <summary>
		/// Gets the expression.
		/// </summary>
		Expression<Func<TItem, object>>? Expression { get; }

		/// <summary>
		/// Gets the compiled <see cref="Expression"/>.
		/// </summary>
		/// <returns>The compiled expression as a delegate.</returns>
		/// <remarks>
		/// This is a method rather than a property because of an issue with MVC model binding in ASP.NET Core.
		/// When reading the property value, the model validation code was throwing an exception and the only way to workaround
		/// that was to make this a method.
		/// </remarks>
		Func<TItem, object>? GetDelegate();
	}

	/// <summary>
	/// An abstraction representing a data expression.
	/// </summary>
	public interface IDataExpression
	{
		/// <summary>
		/// Gets the path of the member.
		/// </summary>
		string? MemberPath { get; }
	}
}