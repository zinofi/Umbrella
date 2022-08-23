// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Security.Claims;

namespace Umbrella.AppFramework.Security.Messages;

public class AuthenticationStateChangedMessage : ValueChangedMessage<ClaimsPrincipal>
{
	public AuthenticationStateChangedMessage(ClaimsPrincipal value)
		: base(value)
	{
	}
}