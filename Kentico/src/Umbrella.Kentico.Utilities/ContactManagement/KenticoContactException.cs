using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Kentico.Utilities.Exceptions;

namespace Umbrella.Kentico.Utilities.ContactManagement
{
	public sealed class KenticoContactException : UmbrellaKenticoException
	{
		internal KenticoContactException()
		{
		}

		internal KenticoContactException(string message)
			: base(message)
		{
		}

		internal KenticoContactException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}