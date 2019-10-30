using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.WebUtilities.Exceptions
{
	/// <summary>
	/// Represents a generic web exception thrown from a class in one of the Umbrella web libraries. This class serves as the base exception
	/// for more specific Umbrella web exceptions thrown from other Umbrella web libraries.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Exceptions.UmbrellaException" />
	public class UmbrellaWebException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebException"/> class.
		/// </summary>
		public UmbrellaWebException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaWebException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaWebException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		public UmbrellaWebException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}