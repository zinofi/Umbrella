using System;
using System.Runtime.Serialization;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Remote.Exceptions
{
	public sealed class UmbrellaHttpServiceAccessException : UmbrellaException
	{
		internal UmbrellaHttpServiceAccessException()
		{
		}

		internal UmbrellaHttpServiceAccessException(string message)
			: base(message)
		{
		}

		internal UmbrellaHttpServiceAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}