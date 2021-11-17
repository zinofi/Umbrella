// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Umbrella.AppFramework.Shared.Security.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="ClaimsPrincipal"/> type.
	/// </summary>
	public static class ClaimPrincipalExtensions
	{
		/// <summary>
		/// Gets the file access token stored as a claim on the specified <paramref name="principal"/> with type <see cref="UmbrellaAppClaimType.FileAccessToken"/>.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <returns>The file access token, if it exists; otherwise <see langword="null"/>.</returns>
		public static string? GetFileAccessToken(this ClaimsPrincipal principal) => principal.FindFirst(UmbrellaAppClaimType.FileAccessToken)?.Value;
	}
}