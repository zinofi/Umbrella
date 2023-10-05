// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;
using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Security.Options;

/// <summary>
/// Options for use with the <see cref="UmbrellaClaimsUserAccessor{TUserId, TRole}" /> type. 
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class UmbrellaClaimsUserAccessorOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the type of the name identifier claim. Defaults to <see cref="ClaimTypes.NameIdentifier"/>.
	/// </summary>
	public string NameIdentifierClaimType { get; set; } = ClaimTypes.NameIdentifier;

	/// <summary>
	/// Gets or sets the role claim type identifier. Defaults to <see cref="ClaimTypes.Role"/>.
	/// </summary>
	public string RoleClaimTypeIdentifier { get; set; } = ClaimTypes.Role;

	/// <inheritdoc />
	public void Sanitize()
	{
		NameIdentifierClaimType = NameIdentifierClaimType.Trim();
		RoleClaimTypeIdentifier = RoleClaimTypeIdentifier.Trim();
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrWhiteSpace(NameIdentifierClaimType);
		Guard.IsNotNullOrWhiteSpace(RoleClaimTypeIdentifier);
	}
}