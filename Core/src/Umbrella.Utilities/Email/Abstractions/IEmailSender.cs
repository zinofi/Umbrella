﻿using Umbrella.Utilities.Email.Options;

namespace Umbrella.Utilities.Email.Abstractions;

// TODO: Swap Attachment to use a custom class called something like EmailAttachment in order to remove the dependency on System.Net.Mail
// in cases where we want to use a different implementation.

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
	/// <param name="attachments">The email attachements.</param>
	/// <param name="ccList">The list of email addresses to be added as CCs.</param>
	/// <param name="bccList">The list of email addresses to be added as BCCs.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable task which will complete when the email has been sent.</returns>
	Task SendEmailAsync(string email, string subject, string body, string? fromAddress = null, IEnumerable<EmailAttachment>? attachments = null, IEnumerable<string>? ccList = null, IEnumerable<string>? bccList = null, CancellationToken cancellationToken = default);
}