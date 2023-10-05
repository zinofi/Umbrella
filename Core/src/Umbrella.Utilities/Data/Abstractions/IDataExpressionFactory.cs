namespace Umbrella.Utilities.Data.Abstractions;

/// <summary>
/// A factory used to create instance of <see cref="IDataExpression"/>.
/// </summary>
public interface IDataExpressionFactory
{
	/// <summary>
	/// Creates a data expression for the specified <paramref name="elementType"/> and <paramref name="descriptor"/>.
	/// </summary>
	/// <typeparam name="TDescriptor">The type of the descriptor.</typeparam>
	/// <param name="elementType">Type of the element.</param>
	/// <param name="descriptor">The descriptor.</param>
	/// <returns>The data expression.</returns>
	IDataExpression? Create<TDescriptor>(Type elementType, TDescriptor descriptor) where TDescriptor : IDataExpressionDescriptor;
}