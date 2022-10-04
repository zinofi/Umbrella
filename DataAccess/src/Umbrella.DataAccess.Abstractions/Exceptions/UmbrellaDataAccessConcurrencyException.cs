// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace Umbrella.DataAccess.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that occurs during data access when a data concurrency issue arises.
/// </summary>
/// <seealso cref="UmbrellaDataAccessException" />
public class UmbrellaDataAccessConcurrencyException : UmbrellaDataAccessException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessConcurrencyException"/> class.
	/// </summary>
	public UmbrellaDataAccessConcurrencyException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataAccessConcurrencyException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataAccessConcurrencyException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataAccessConcurrencyException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaDataAccessConcurrencyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}