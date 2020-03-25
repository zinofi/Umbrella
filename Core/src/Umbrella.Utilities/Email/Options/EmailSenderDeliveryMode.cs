namespace Umbrella.Utilities.Email.Options
{
	/// <summary>
	/// Specifies the delivery method for sending emails.
	/// </summary>
	public enum EmailSenderDeliveryMode
	{
		/// <summary>
		/// Email is sent through the network to an SMTP server.
		/// </summary>
		Network = 0,

		/// <summary>
		/// Email is copied to a specified directory on disk.
		/// </summary>
		SpecifiedPickupDirectory = 1
	}
}