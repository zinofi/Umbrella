// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Umbrella.AppFramework.Utilities.Messages;

public class AppUpdateStateChangedMessage : ValueChangedMessage<(bool updateRequired, string message)>
{
	public AppUpdateStateChangedMessage((bool updateRequired, string message) value) : base(value)
	{
	}
}