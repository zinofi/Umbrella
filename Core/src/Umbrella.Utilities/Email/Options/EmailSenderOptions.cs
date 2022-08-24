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

	/// <inheritdoc />
	public void Sanitize()
	{
		DeliveryMethod = DeliveryMethod < 0 ? EmailSenderDeliveryMode.Network : DeliveryMethod;
		DefaultFromAddress = DefaultFromAddress?.Trim();
		PickupDirectoryLocation = PickupDirectoryLocation?.Trim();
		Host = Host?.Trim();
		UserName = UserName?.Trim();
		Password = Password?.Trim();
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown if <see cref="DefaultFromAddress"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <see cref="DefaultFromAddress"/> is empty or whitespace.</exception>
	public void Validate()
	{
		Guard.IsNotNullOrWhiteSpace(DefaultFromAddress, nameof(DefaultFromAddress));

		switch (DeliveryMethod)
		{
			case EmailSenderDeliveryMode.Network:
				Guard.IsNotNullOrWhiteSpace(Host, nameof(Host));

				if (!string.IsNullOrWhiteSpace(UserName))
					Guard.IsNotNullOrWhiteSpace(Password, nameof(Password));
				break;
			case EmailSenderDeliveryMode.SpecifiedPickupDirectory:
				Guard.IsNotNullOrWhiteSpace(PickupDirectoryLocation, nameof(PickupDirectoryLocation));
				break;
		}
	}
}