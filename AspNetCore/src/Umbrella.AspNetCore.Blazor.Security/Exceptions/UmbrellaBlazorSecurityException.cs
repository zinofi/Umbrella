using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Security.Exceptions;

/// <summary>
/// Represents an exception thrown from a class in the <see cref="Security"/> library.
/// </summary>
/// <seealso cref="UmbrellaException" />
[Serializable]
public class UmbrellaBlazorSecurityException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorSecurityException"/> class.
	/// </summary>
	public UmbrellaBlazorSecurityException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorSecurityException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaBlazorSecurityException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorSecurityException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaBlazorSecurityException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorSecurityException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaBlazorSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}