// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Globalization;
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

		public static DateTime? GetRefreshTokenExpiration(this ClaimsPrincipal principal)
		{
			string? strExpiration = principal.FindFirst(UmbrellaAppClaimType.RefreshTokenExpiration)?.Value;

			return DateTime.TryParse(strExpiration, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result)
				? result
				: null;
		}

		public static TAppRole GetPrimaryRole<TAppRole>(this ClaimsPrincipal principal)
			where TAppRole : struct, Enum
		{
			string? value = principal.FindFirst(UmbrellaAppClaimType.PrimaryRole)?.Value;

			if (string.IsNullOrWhiteSpace(value))
				throw new Exception("The current principal has no primary role.");

			if (!Enum.TryParse(value, out TAppRole roleType))
				throw new Exception($"The current principal does not have a valid primary role: {value}");

			return roleType;
		}
	}
}