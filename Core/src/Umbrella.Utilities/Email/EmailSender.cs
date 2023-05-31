// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Net.Mail;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Email.Abstractions;
using Umbrella.Utilities.Email.Options;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Email;

/// <summary>
/// An implementation of the <see cref="IEmailSender" /> abstraction that either sends emails
/// using a configured SMTP server or saves them to a folder on disk as .eml files. Configuration is specified using
/// the singleton <see cref="EmailSenderOptions" /> configured with the application's DI container.
/// </summary>
public class EmailSender : IEmailSender
{
	private readonly ILogger<EmailSender> _logger;
	private readonly EmailSenderOptions _options;

	/// <summary>
	/// Create a new instance.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The options.</param>
	public EmailSender(
		ILogger<EmailSender> logger,
		EmailSenderOptions options)
	{
		_logger = logger;
		_options = options;
	}

	/// <inheritdoc />
	public async Task SendEmailAsync(string email, string subject, string body, string? fromAddress = null, IEnumerable<Attachment>? attachments = null, CancellationToken cancellationToken = default)
	{
		// TODO: Add parameters to allow adding CC and BCC email addresses.

		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(email);
		Guard.IsNotNullOrWhiteSpace(subject);
		Guard.IsNotNullOrWhiteSpace(body);

		try
		{
			using var client = new SmtpClient();

			switch (_options.DeliveryMethod)
			{
				case EmailSenderDeliveryMode.Network:
					{
						client.DeliveryMethod = SmtpDeliveryMethod.Network;
						client.Host = _options.Host;
						client.Port = _options.Port;
						client.EnableSsl = _options.SecureServerConnection;

						if (!string.IsNullOrWhiteSpace(_options.UserName))
							client.Credentials = new NetworkCredential { UserName = _options.UserName, Password = _options.Password };

						if (_options.RedirectRecipientEmailsList.Count > 0 && !_options.EmailRecipientDomainWhiteList.Any(x => email.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
						{
							email = _options.GetRedirectRecipientEmails();
						}
						else if (_options.EmailRecipientDomainWhiteList.Count > 0 && !_options.EmailRecipientDomainWhiteList.Any(x => email.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
						{
							return;
						}

						break;
					}
				case EmailSenderDeliveryMode.SpecifiedPickupDirectory:
					{
						client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
						client.PickupDirectoryLocation = _options.PickupDirectoryLocation;
						break;
					}
				default:
					throw new NotSupportedException($"Only {nameof(SmtpDeliveryMethod.Network)} and {nameof(SmtpDeliveryMethod.SpecifiedPickupDirectory)} are supported as delivery methods.");
			}

			using var message = new MailMessage(fromAddress?.TrimToLowerInvariant() ?? _options.DefaultFromAddress, email)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			attachments?.ForEach(message.Attachments.Add);

			await client.SendMailAsync(message).ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { email, subject, body, fromAddress }))
		{
			throw new UmbrellaException("There has been a problem sending the email.", exc);
		}
	}
}