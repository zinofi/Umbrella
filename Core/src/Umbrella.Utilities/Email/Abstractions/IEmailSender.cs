using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Email.Abstractions
{
	/// <summary>
	/// A service used to send emails.
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		/// Sends the email to the specified email address.
		/// </summary>
		/// <param name="email">The email address.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="body">The body of the email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable task which will complete when the email has been sent.</returns>
		Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default);
	}
}