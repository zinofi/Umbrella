using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Http.Exceptions;

/// <summary>
/// Represents an error that occurs during execution of a HTTP Request.
/// </summary>
/// <seealso cref="UmbrellaException" />
[Serializable]
public class UmbrellaHttpServiceAccessException : UmbrellaException
{
	internal UmbrellaHttpServiceAccessException()
	{
	}

	internal UmbrellaHttpServiceAccessException(string message)
		: base(message)
	{
	}

	internal UmbrellaHttpServiceAccessException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaHttpServiceAccessException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
	protected UmbrellaHttpServiceAccessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}