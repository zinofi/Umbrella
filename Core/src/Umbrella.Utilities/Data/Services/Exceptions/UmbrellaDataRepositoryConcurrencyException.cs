using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Data.Services.Exceptions;

/// <summary>
/// Represents a concurrency error that occurs during execution of a data operation.
/// </summary>
/// <remarks>By design, this does not derive from <see cref="UmbrellaDataServiceException"/> and as such will have to be handled separately.</remarks>
/// <seealso cref="UmbrellaConcurrencyException" />
public class UmbrellaDataServiceConcurrencyException : UmbrellaConcurrencyException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceConcurrencyException"/> class.
	/// </summary>
	public UmbrellaDataServiceConcurrencyException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataServiceConcurrencyException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataServiceConcurrencyException(string message, Exception innerException) : base(message, innerException)
	{
	}
}