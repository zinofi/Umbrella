using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Xamarin.Exceptions
{
	/// <summary>
	/// Represents an exception in the Umbrella.Xamarin library.
	/// </summary>
	/// <seealso cref="UmbrellaException" />
	public class UmbrellaXamarinException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaXamarinException"/> class.
		/// </summary>
		public UmbrellaXamarinException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaXamarinException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaXamarinException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaXamarinException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaXamarinException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaXamarinException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaXamarinException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}