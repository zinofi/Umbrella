// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Exceptions;

/// <summary>
/// Represents an exception thrown from a class in the <see cref="Blazor"/> library.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class UmbrellaBlazorException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorException"/> class.
	/// </summary>
	public UmbrellaBlazorException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaBlazorException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaBlazorException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaBlazorException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}