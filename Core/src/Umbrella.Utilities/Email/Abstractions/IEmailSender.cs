using Umbrella.Utilities.Email.Options;

namespace Umbrella.Utilities.Email.Abstractions;

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
	/// <param name="fromAddress">The email address of the sender. If this is not specified, the default email address will be automatically set to the default value <see cref="EmailSenderOptions.DefaultFromAddress"/>.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable task which will complete when the email has been sent.</returns>
	Task SendEmailAsync(string email, string subject, string body, string? fromAddress = null, CancellationToken cancellationToken = default);
}