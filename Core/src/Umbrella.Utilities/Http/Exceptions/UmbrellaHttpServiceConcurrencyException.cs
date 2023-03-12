using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Http.Exceptions;

/// <summary>
/// Represents a concurrency error that occurs during execution of a HTTP Request.
/// </summary>
/// <remarks>By design, this does not derive from <see cref="UmbrellaHttpServiceAccessException"/> and as such will have to be handled separately.</remarks>
/// <seealso cref="UmbrellaConcurrencyException" />
public class UmbrellaHttpServiceConcurrencyException : UmbrellaConcurrencyException
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
}