using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Exceptions
{
	/// <summary>
	/// Represents an exception thrown from a class in the <see cref="Umbrella.AspNetCore.Blazor"/> library.
	/// </summary>
	/// <seealso cref="UmbrellaException" />
	public class UmbrellaWebComponentException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebComponentException"/> class.
		/// </summary>
		public UmbrellaWebComponentException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebComponentException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaWebComponentException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebComponentException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaWebComponentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaWebComponentException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaWebComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}