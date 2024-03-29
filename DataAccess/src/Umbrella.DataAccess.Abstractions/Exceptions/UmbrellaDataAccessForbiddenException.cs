﻿using System.Runtime.Serialization;

namespace Umbrella.DataAccess.Abstractions.Exceptions;

/// <summary>
/// Represents an exception thrown during data access when the current user is forbidden to access a requested resource.
/// </summary>
/// <seealso cref="UmbrellaDataAccessException" />
[Serializable]
public class UmbrellaDataAccessForbiddenException : UmbrellaDataAccessException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessForbiddenException"/> class.
	/// </summary>
	public UmbrellaDataAccessForbiddenException()
		: base("The current user is forbidden from accessing the specified resource.")
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessForbiddenException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataAccessForbiddenException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessForbiddenException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataAccessForbiddenException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessForbiddenException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaDataAccessForbiddenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}