using Umbrella.Utilities.Exceptions;

namespace Umbrella.Legacy.Utilities.Configuration.Exceptions
{
	/// <summary>
	/// Represents an exception thrown during a configuration error with an Umbrella config section.
	/// </summary>
	/// <seealso cref="UmbrellaException" />
	public class UmbrellaConfigurationException : UmbrellaException
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaConfigurationException"/> class.
		/// </summary>
		public UmbrellaConfigurationException()
            : base("A generic Umbrella configuration section has occurred.")
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UmbrellaConfigurationException(string message)
            : base(message)
        {
        }
    }
}