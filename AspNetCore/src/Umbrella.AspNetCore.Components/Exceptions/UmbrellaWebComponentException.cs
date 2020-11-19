using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AspNetCore.Components.Exceptions
{
	public class UmbrellaWebComponentException : UmbrellaException
	{
		public UmbrellaWebComponentException()
		{
		}

		public UmbrellaWebComponentException(string message) : base(message)
		{
		}

		public UmbrellaWebComponentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UmbrellaWebComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}