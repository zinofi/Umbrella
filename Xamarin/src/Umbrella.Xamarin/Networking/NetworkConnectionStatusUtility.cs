// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Networking.Abstractions;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Networking
{
	public class NetworkConnectionStatusUtility : INetworkConnectionStatusUtility
	{
		/// <inheritdoc />
		public bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
	}
}