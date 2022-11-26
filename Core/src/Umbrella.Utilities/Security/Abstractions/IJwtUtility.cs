// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Umbrella.Utilities.Security.Abstractions;

/// <summary>
/// A utility class containing useful methods to help manage JSON Web Tokens.
/// </summary>
public interface IJwtUtility
{
	/// <summary>
	/// Parses the claims from the specified JSON Web Token.
	/// </summary>
	/// <param name="jwt">The JWT.</param>
	/// <param name="roleClaimType">Type of the role claim.</param>
	/// <returns>The parsed claims.</returns>
	IReadOnlyCollection<Claim> ParseClaimsFromJwt(string jwt, string roleClaimType = ClaimTypes.Role);
}