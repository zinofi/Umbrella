// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AspNetCore.Blazor.Security.Options;

/// <summary>
/// Options for use with the <see cref="ClaimsPrincipalAuthenticationStateProvider"/>.
/// </summary>
public class ClaimsPrincipalAuthenticationStateProviderOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether Application Insights should have the authentication context
	/// set to the currently authenticated user.
	/// </summary>
	public bool IsApplicationInsightsEnabled { get; set; }
}