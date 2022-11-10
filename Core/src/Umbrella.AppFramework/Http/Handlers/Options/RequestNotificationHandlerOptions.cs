// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Http.Handlers.Options;

/// <summary>
/// Options for use with the <see cref="RequestNotificationHandler"/> delegating handler.
/// </summary>
public class RequestNotificationHandlerOptions
{
	/// <summary>
	/// Gets the paths that should be ignored by the <see cref="RequestNotificationHandler"/>.
	/// </summary>
	public HashSet<RequestNotificationHandlerExclusion> Exclusions { get; } = new HashSet<RequestNotificationHandlerExclusion>();
}