using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.TypeScript.Exceptions
{
	/// <summary>
	/// An Umbrella exception used to represent exceptions thrown from TypeScript generation infrastructure.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Exceptions.UmbrellaException" />
	public class UmbrellaTypeScriptException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaTypeScriptException"/> class.
		/// </summary>
		public UmbrellaTypeScriptException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaTypeScriptException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaTypeScriptException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaTypeScriptException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaTypeScriptException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaTypeScriptException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaTypeScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}