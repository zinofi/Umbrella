// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Networking.Abstractions;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Networking;

/// <summary>
/// A utility used to determine if there is a network connection.
/// </summary>
/// <seealso cref="INetworkConnectionStatusUtility" />
public class NetworkConnectionStatusUtility : INetworkConnectionStatusUtility
{
	/// <inheritdoc />
	public bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
}