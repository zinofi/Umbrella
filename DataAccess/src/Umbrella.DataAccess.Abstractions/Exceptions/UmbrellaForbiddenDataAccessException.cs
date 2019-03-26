using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public sealed class UmbrellaForbiddenDataAccessException : UmbrellaDataAccessException
	{
		internal UmbrellaForbiddenDataAccessException()
			: base("The current user is forbidden from accessing the specified resource.")
		{
		}

		internal UmbrellaForbiddenDataAccessException(string message)
			: base(message)
		{
		}

		internal UmbrellaForbiddenDataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}