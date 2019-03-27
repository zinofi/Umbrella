using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Kentico.Utilities.Exceptions
{
	public class UmbrellaKenticoException : UmbrellaException
	{
		internal UmbrellaKenticoException()
		{
		}

		internal UmbrellaKenticoException(string message)
			: base(message)
		{
		}

		internal UmbrellaKenticoException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}