using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Options.Exceptions;

/// <summary>
/// Represents an exception thrown when sanitizing or validation Umbrella options.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class UmbrellaOptionsException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaOptionsException"/> class.
	/// </summary>
	public UmbrellaOptionsException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaOptionsException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaOptionsException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaOptionsException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaOptionsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaOptionsException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaOptionsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}