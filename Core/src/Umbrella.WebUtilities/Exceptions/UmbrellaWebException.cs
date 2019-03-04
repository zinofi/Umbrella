using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.WebUtilities.Exceptions
{
	public class UmbrellaWebException : UmbrellaException
	{
		public UmbrellaWebException()
		{
		}

		public UmbrellaWebException(string message)
			: base(message)
		{
		}

		public UmbrellaWebException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public UmbrellaWebException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}