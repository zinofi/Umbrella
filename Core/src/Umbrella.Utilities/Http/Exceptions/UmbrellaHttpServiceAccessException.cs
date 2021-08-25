using System;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Http.Exceptions
{
	/// <summary>
	/// Represents an error that occurs during execution of a HTTP Request.
	/// </summary>
	/// <seealso cref="UmbrellaException" />
	public class UmbrellaHttpServiceAccessException : UmbrellaException
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