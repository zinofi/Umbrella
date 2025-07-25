using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Data.Repositories.Exceptions;

/// <summary>
/// Represents a concurrency error that occurs during execution of a data operation.
/// </summary>
/// <remarks>By design, this does not derive from <see cref="UmbrellaDataRepositoryException"/> and as such will have to be handled separately.</remarks>
/// <seealso cref="UmbrellaConcurrencyException" />
public class UmbrellaDataRepositoryConcurrencyException : UmbrellaConcurrencyException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryConcurrencyException"/> class.
	/// </summary>
	public UmbrellaDataRepositoryConcurrencyException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UmbrellaDataRepositoryConcurrencyException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDataRepositoryConcurrencyException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public UmbrellaDataRepositoryConcurrencyException(string message, Exception innerException) : base(message, innerException)
	{
	}
}