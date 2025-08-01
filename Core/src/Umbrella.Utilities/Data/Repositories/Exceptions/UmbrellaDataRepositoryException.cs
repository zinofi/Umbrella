﻿using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Data.Repositories.Exceptions;

/// <summary>
/// Represents an error that occurs during execution of a data operation.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class UmbrellaDataRepositoryException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryException"/> class.
	/// </summary>
	public UmbrellaDataRepositoryException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataRepositoryException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataRepositoryException(string message, Exception innerException) : base(message, innerException)
	{
	}
}