// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Shared.Security
{
	/// <summary>
	/// Built-in claim types used by the Umbrella AppFramework.
	/// </summary>
	public static class UmbrellaAppClaimType
	{
		private const string ClaimPrefix = "Umbrella.";

		/// <summary>
		/// The file access token claim type.
		/// </summary>
		public const string FileAccessToken = ClaimPrefix + nameof(FileAccessToken);

		public const string RefreshToken = ClaimPrefix + nameof(RefreshToken);
		public const string RefreshTokenExpiration = ClaimPrefix + nameof(RefreshTokenExpiration);
		public const string PrimaryRole = ClaimPrefix + nameof(PrimaryRole);
	}
}