using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	// TODO: V3 - Prefix name with Umbrella
	public class DataAccessConcurrencyException : UmbrellaDataAccessException
	{
		public DataAccessConcurrencyException()
		{
		}

		public DataAccessConcurrencyException(string message)
			: base(message)
		{
		}

		public DataAccessConcurrencyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DataAccessConcurrencyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}