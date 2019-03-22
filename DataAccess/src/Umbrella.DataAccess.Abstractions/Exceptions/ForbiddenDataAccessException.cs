using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class ForbiddenDataAccessException : DataAccessException
	{
		public ForbiddenDataAccessException()
			: base("The current user is forbidden from accessing the specified resource.")
		{
		}

		public ForbiddenDataAccessException(string message)
			: base(message)
		{
		}

		public ForbiddenDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ForbiddenDataAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}