using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Http.Exceptions;

/// <summary>
/// Represents a concurrency error that occurs during execution of a HTTP Request.
/// </summary>
/// <remarks>By design, this does not derive from <see cref="UmbrellaHttpServiceAccessException"/> and as such will have to be handled separately.</remarks>
/// <seealso cref="UmbrellaConcurrencyException" />
[Serializable]
public sealed class UmbrellaHttpServiceConcurrencyException : UmbrellaConcurrencyException
{
	internal UmbrellaHttpServiceConcurrencyException()
	{
	}

	internal UmbrellaHttpServiceConcurrencyException(string message)
		: base(message)
	{
	}

	internal UmbrellaHttpServiceConcurrencyException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaHttpServiceConcurrencyException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
	private UmbrellaHttpServiceConcurrencyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}