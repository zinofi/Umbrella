﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace Umbrella.Utilities.Exceptions;

/// <summary>
/// Represents a concurrency exception thrown from a class in one of the Umbrella libraries. This class serves as the base exception
/// for more specific Umbrella concurrency exceptions thrown from other Umbrella libraries.
/// </summary>
/// <seealso cref="UmbrellaException" />
[Serializable]
public class UmbrellaConcurrencyException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaConcurrencyException"/> class.
	/// </summary>
	public UmbrellaConcurrencyException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaConcurrencyException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaConcurrencyException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaConcurrencyException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}