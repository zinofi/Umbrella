using System;

namespace Umbrella.Utilities.Http.Exceptions
{
	/// <summary>
	/// Represents a conurrency error that occurs during execution of a HTTP Request.
	/// </summary>
	/// <seealso cref="UmbrellaHttpServiceAccessException" />
	public class UmbrellaHttpServiceConcurrencyException : UmbrellaHttpServiceAccessException
	{
		internal UmbrellaHttpServiceConcurrencyException()
		{
		}

		internal UmbrellaHttpServiceConcurrencyException(string message)
			: base(message)
		{
		}

		internal UmbrellaHttpServiceConcurrencyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}