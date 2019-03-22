using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public class UmbrellaForbiddenDataAccessException : UmbrellaDataAccessException
	{
		public UmbrellaForbiddenDataAccessException()
			: base("The current user is forbidden from accessing the specified resource.")
		{
		}

		public UmbrellaForbiddenDataAccessException(string message)
			: base(message)
		{
		}

		public UmbrellaForbiddenDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UmbrellaForbiddenDataAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}