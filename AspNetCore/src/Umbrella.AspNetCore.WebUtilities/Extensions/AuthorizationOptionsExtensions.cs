// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Umbrella.AspNetCore.WebUtilities.Security.Policies;

namespace Umbrella.AspNetCore.WebUtilities.Extensions;

/// <summary>
/// Extension methods for <see cref="AuthorizationOptions"/> instances.
/// </summary>
public static class AuthorizationOptionsExtensions
{
	/// <summary>
	/// Adds the core policies.
	/// </summary>
	/// <param name="options">The options.</param>
	public static void AddCorePolicies(this AuthorizationOptions options)
	{
		options.AddPolicy(CorePolicyNames.Create, x => x.Requirements.Add(CoreItemOperations.Create));
		options.AddPolicy(CorePolicyNames.Read, x => x.Requirements.Add(CoreItemOperations.Read));
		options.AddPolicy(CorePolicyNames.Update, x => x.Requirements.Add(CoreItemOperations.Update));
		options.AddPolicy(CorePolicyNames.Delete, x => x.Requirements.Add(CoreItemOperations.Delete));
	}
}