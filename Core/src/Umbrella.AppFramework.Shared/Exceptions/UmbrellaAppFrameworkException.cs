using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AppFramework.Shared.Exceptions;

/// <summary>
/// Represents an exception thrown from a class in the <see cref="Shared"/> library.
/// </summary>
/// <seealso cref="UmbrellaException" />
[Serializable]
public class UmbrellaAppFrameworkSharedException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAppFrameworkSharedException"/> class.
	/// </summary>
	public UmbrellaAppFrameworkSharedException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAppFrameworkSharedException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaAppFrameworkSharedException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAppFrameworkSharedException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaAppFrameworkSharedException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAppFrameworkSharedException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaAppFrameworkSharedException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}