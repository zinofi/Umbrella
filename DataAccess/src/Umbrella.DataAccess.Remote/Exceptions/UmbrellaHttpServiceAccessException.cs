using System;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DataAccess.Remote.Exceptions
{
	/// <summary>
	/// Represents an error that occurs during execution of a HTTP Request.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Exceptions.UmbrellaException" />
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