// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Mapping.Abstractions;

namespace Umbrella.Utilities.Exceptions;

/// <summary>
/// Represents an object mapping exception thrown from a class in one of the Umbrella libraries.
/// Primarily used by <see cref="IUmbrellaMapper"/> implementations.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class UmbrellaMappingException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMappingException"/> class.
	/// </summary>
	public UmbrellaMappingException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMappingException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaMappingException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMappingException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaMappingException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMappingException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}