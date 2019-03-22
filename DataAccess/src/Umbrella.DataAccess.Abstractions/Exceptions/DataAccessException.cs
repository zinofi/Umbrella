using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class DataAccessException : Exception
	{
		public DataAccessException()
		{
		}

		public DataAccessException(string message)
			: base(message)
		{
		}

		public DataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DataAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}