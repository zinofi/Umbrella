using System;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	/// <summary>
	/// Represents an exception thrown during data access when the current user is forbidden to access a requested resource.
	/// </summary>
	/// <seealso cref="Umbrella.DataAccess.Abstractions.Exceptions.UmbrellaDataAccessException" />
	public sealed class UmbrellaForbiddenDataAccessException : UmbrellaDataAccessException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaForbiddenDataAccessException"/> class.
		/// </summary>
		internal UmbrellaForbiddenDataAccessException()
			: base("The current user is forbidden from accessing the specified resource.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaForbiddenDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		internal UmbrellaForbiddenDataAccessException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaForbiddenDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		internal UmbrellaForbiddenDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}