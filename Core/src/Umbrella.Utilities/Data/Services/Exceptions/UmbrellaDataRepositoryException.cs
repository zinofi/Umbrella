using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Data.Services.Exceptions;

/// <summary>
/// Represents an error that occurs during execution of a data operation.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class UmbrellaDataServiceException : UmbrellaException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceException"/> class.
	/// </summary>
	public UmbrellaDataServiceException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataServiceException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataServiceException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataServiceException(string message, Exception innerException) : base(message, innerException)
	{
	}
}