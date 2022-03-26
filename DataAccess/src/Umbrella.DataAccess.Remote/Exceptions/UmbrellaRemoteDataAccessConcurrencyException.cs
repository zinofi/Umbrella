// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Remote.Exceptions
{
	/// <summary>
	/// Represents a concurrency error that occurs during execution of a remote data access operation.
	/// </summary>
	/// <remarks>By design, this does not derive from <see cref="UmbrellaRemoteDataAccessException"/> and as such will have to be handled separately.</remarks>
	/// <seealso cref="UmbrellaConcurrencyException" />
	public class UmbrellaRemoteDataAccessConcurrencyException : UmbrellaConcurrencyException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessConcurrencyException"/> class.
		/// </summary>
		public UmbrellaRemoteDataAccessConcurrencyException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessConcurrencyException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaRemoteDataAccessConcurrencyException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessConcurrencyException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public UmbrellaRemoteDataAccessConcurrencyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaRemoteDataAccessConcurrencyException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		protected UmbrellaRemoteDataAccessConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}