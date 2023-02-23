// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Utilities.Email.Options;

/// <summary>
/// The options for the <see cref="EmailSender"/>.
/// </summary>
public class EmailSenderOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// The method of delivery. Defaults to <see cref="EmailSenderDeliveryMode.Network" />.
	/// </summary>
	public EmailSenderDeliveryMode DeliveryMethod { get; set; } = EmailSenderDeliveryMode.Network;

	/// <summary>
	/// The default sender address.
	/// </summary>
	public string? DefaultFromAddress { get; set; }

	/// <summary>
	/// The path to the folder on disk where emails will be saved when <see cref="DeliveryMethod"/> is set
	/// to <see cref="EmailSenderDeliveryMode.SpecifiedPickupDirectory"/>.
	/// </summary>
	public string? PickupDirectoryLocation { get; set; }

	/// <summary>
	/// The address of the SMTP server.
	/// </summary>
	public string? Host { get; set; }

	/// <summary>
	/// The port used to connect to the SMTP server. Defaults to 25.
	/// </summary>
	public int Port { get; set; } = 25;

	/// <summary>
	/// The username used to connect to the SMTP server.
	/// </summary>
	public string? UserName { get; set; }

	/// <summary>
	/// The password used to connect to the SMTP server.
	/// </summary>
	public string? Password { get; set; }

	/// <summary>
	/// Specifies whether or not the SMTP server connection is secured. Defaults to <see langword="false" />.
	/// </summary>
	public bool SecureServerConnection { get; set; }

	/// <summary>
	/// Gets or sets the email recipient white list.
	/// </summary>
	/// <remarks>
	/// When specified, outbound emails will only be permitted to be sent to these recipients.
	/// All other recipients will have their emails redirected to all email addresses specified using the
	/// <see cref="RedirectRecipientEmailsList"/>.
	/// </remarks>
	public IReadOnlyCollection<string> EmailRecipientWhiteList { get; set; } = Array.Empty<string>();

	/// <summary>
	/// Gets or sets the redirect recipient emails list.
	/// </summary>
	/// <remarks>
	/// When specified, all outbound emails, except those specified on the <see cref="EmailRecipientWhiteList"/>
	/// will be redirected to these email addresses.
	/// </remarks>
	public IReadOnlyCollection<string> RedirectRecipientEmailsList { get; set; } = Array.Empty<string>();

	/// <inheritdoc />
	public void Sanitize()
	{
		DeliveryMethod = DeliveryMethod < 0 ? EmailSenderDeliveryMode.Network : DeliveryMethod;
		DefaultFromAddress = DefaultFromAddress?.Trim();
		PickupDirectoryLocation = PickupDirectoryLocation?.Trim();
		Host = Host?.Trim();
		UserName = UserName?.Trim();
		Password = Password?.Trim();
		EmailRecipientWhiteList = EmailRecipientWhiteList.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
		RedirectRecipientEmailsList = RedirectRecipientEmailsList.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrWhiteSpace(DefaultFromAddress);
		Guard.IsNotNull(EmailRecipientWhiteList);
		Guard.IsNotNull(RedirectRecipientEmailsList);

		switch (DeliveryMethod)
		{
			case EmailSenderDeliveryMode.Network:
				Guard.IsNotNullOrWhiteSpace(Host);

				if (!string.IsNullOrWhiteSpace(UserName))
					Guard.IsNotNullOrWhiteSpace(Password);
				break;
			case EmailSenderDeliveryMode.SpecifiedPickupDirectory:
				Guard.IsNotNullOrWhiteSpace(PickupDirectoryLocation);
				break;
		}
	}
}