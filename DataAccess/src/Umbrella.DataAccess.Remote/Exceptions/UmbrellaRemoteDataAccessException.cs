// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Remote.Exceptions
{
	/// <summary>
	/// Represents an error that occurs during execution of a remote data access operation.
	/// </summary>
	/// <seealso cref="UmbrellaException" />
	public class UmbrellaRemoteDataAccessException : UmbrellaException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessException"/> class.
		/// </summary>
		public UmbrellaRemoteDataAccessException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaRemoteDataAccessException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaRemoteDataAccessException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaRemoteDataAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}