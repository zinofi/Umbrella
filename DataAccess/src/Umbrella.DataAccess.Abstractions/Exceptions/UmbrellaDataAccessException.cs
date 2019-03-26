using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class UmbrellaDataAccessException : UmbrellaException
	{
		public UmbrellaDataAccessException()
		{
		}

		public UmbrellaDataAccessException(string message)
			: base(message)
		{
		}

		public UmbrellaDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UmbrellaDataAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}