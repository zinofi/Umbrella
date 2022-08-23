// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging.Messages;
using Umbrella.AppFramework.Utilities.Enumerations;

namespace Umbrella.AppFramework.Utilities.Messages;

public class LoadingScreenStateChangedMessage : ValueChangedMessage<LoadingScreenState>
{
	public LoadingScreenStateChangedMessage(LoadingScreenState value) : base(value)
	{
	}
}