// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Security.Claims;

namespace Umbrella.AppFramework.Security.Messages;

/// <summary>
/// A message that signals when the authentication state for the current <see cref="ClaimsPrincipal"/> has changed.
/// </summary>
/// <seealso cref="ValueChangedMessage{ClaimsPrincipal}" />
public class AuthenticationStateChangedMessage : ValueChangedMessage<ClaimsPrincipal>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticationStateChangedMessage"/> class.
	/// </summary>
	/// <param name="value">The value that has changed.</param>
	public AuthenticationStateChangedMessage(ClaimsPrincipal value)
		: base(value)
	{
	}
}