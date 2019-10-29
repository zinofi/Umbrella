using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	/// <summary>
	/// Represents a generic data access exception thrown from one of the Umbrella.DataAccess libraries.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Exceptions.UmbrellaException" />
	public class UmbrellaDataAccessException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDataAccessException"/> class.
		/// </summary>
		public UmbrellaDataAccessException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaDataAccessException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDataAccessException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaDataAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}