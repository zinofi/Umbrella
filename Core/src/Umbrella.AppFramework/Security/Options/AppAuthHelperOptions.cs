// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Security.Options;

/// <summary>
/// Options for use with the <see cref="JwtAppAuthHelper"/> class.
/// </summary>
public class AppAuthHelperOptions
{
	/// <summary>
	/// Gets the post logout action.
	/// </summary>
	public Func<ValueTask>? PostLogoutAction { get; set; }
}