namespace Umbrella.DataAccess.Abstractions.Exceptions;

/// <summary>
/// Represents an exception thrown during data access that results in multiple exceptions.
/// </summary>
/// <seealso cref="UmbrellaDataAccessException" />
public class UmbrellaDataAccessAggregateException : UmbrellaDataAccessException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessAggregateException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerExceptions">The inner exceptions.</param>
	public UmbrellaDataAccessAggregateException(string message, params Exception[] innerExceptions)
		: base(message, new AggregateException(innerExceptions))
	{
	}
}