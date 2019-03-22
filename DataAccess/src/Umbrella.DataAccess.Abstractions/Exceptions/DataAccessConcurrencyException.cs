using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class DataAccessConcurrencyException : DataAccessException
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