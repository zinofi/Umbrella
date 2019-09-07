using System;
using System.Runtime.Serialization;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class UmbrellaDataAccessConcurrencyException : UmbrellaDataAccessException
	{
		public UmbrellaDataAccessConcurrencyException()
		{
		}

		public UmbrellaDataAccessConcurrencyException(string message)
			: base(message)
		{
		}

		public UmbrellaDataAccessConcurrencyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UmbrellaDataAccessConcurrencyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}