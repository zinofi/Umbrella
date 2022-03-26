// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Umbrella.AppFramework.Shared.Enumerations;

namespace Umbrella.AppFramework.Shared.Primitives
{
	/// <summary>
	/// Represents information on a client application.
	/// </summary>
	public readonly struct AppClientInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppClientInfo"/> struct.
		/// </summary>
		/// <param name="id">The identifier of the client app.</param>
		/// <param name="type">The client type, e.g. web</param>
		/// <param name="version">The version of the client app.</param>
		public AppClientInfo(string id, AppClientType type, Version version)
		{
			Id = id;
			Type = type;
			Version = version;
		}

		/// <summary>
		/// Gets the identifier of the client app.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the type of the client app, e.g. web.
		/// </summary>
		public AppClientType Type { get; }

		/// <summary>
		/// Gets the version of the client app.
		/// </summary>
		public Version Version { get; }
	}
}