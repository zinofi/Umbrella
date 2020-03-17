using System.Net.Mail;

namespace Umbrella.Utilities.Email.Options
{
	/// <summary>
	/// The options for the <see cref="EmailSender"/>.
	/// </summary>
	public class EmailSenderOptions
	{
		/// <summary>
		/// The method of delivery. Defaults to <see cref="SmtpDeliveryMethod.Network" />.
		/// <see cref="SmtpDeliveryMethod.PickupDirectoryFromIis" /> is not supported.
		/// </summary>
		public SmtpDeliveryMethod DeliveryMethod { get; set; } = SmtpDeliveryMethod.Network;

		/// <summary>
		/// The sender address.
		/// </summary>
		public string FromAddress { get; set; }

		/// <summary>
		/// The path to the folder on disk where emails will be saved when <see cref="DeliveryMethod"/> is set
		/// to <see cref="SmtpDeliveryMethod.SpecifiedPickupDirectory"/>.
		/// </summary>
		public string PickupDirectoryLocation { get; set; }

		/// <summary>
		/// The address of the SMTP server.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// The port used to connect to the SMTP server. Defaults to 25.
		/// </summary>
		public int Port { get; set; } = 25;

		/// <summary>
		/// The username used to connect to the SMTP server.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// The password used to connect to the SMTP server.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Specifies whether or not the SMTP server connection is secured. Defaults to <see langword="false" />.
		/// </summary>
		public bool SecureServerConnection { get; set; }
	}
}