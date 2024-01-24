using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AzureServiceBus.Exceptions;

/// <summary>
/// Represents errors that occur within the Umbrella.AzureServiceBus library.
/// </summary>
[Serializable]
public class UmbrellaAzureServiceBusException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAzureServiceBusException"/> class.
	/// </summary>
	public UmbrellaAzureServiceBusException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAzureServiceBusException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaAzureServiceBusException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAzureServiceBusException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public UmbrellaAzureServiceBusException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAzureServiceBusException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	protected UmbrellaAzureServiceBusException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}