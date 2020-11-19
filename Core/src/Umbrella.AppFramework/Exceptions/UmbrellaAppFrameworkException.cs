using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.AppFramework.Exceptions
{
	public class UmbrellaAppFrameworkException : UmbrellaException
	{
		public UmbrellaAppFrameworkException()
		{
		}

		public UmbrellaAppFrameworkException(string message) : base(message)
		{
		}

		public UmbrellaAppFrameworkException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UmbrellaAppFrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}