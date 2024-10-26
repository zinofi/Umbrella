using System.Security.Claims;

namespace Umbrella.Utilities.Context.Abstractions;

/// <summary>
/// Used to allow access to the claims of the current user.
/// </summary>
[Obsolete("Please use ClaimsPrincipal.Current to access.", true)]
public interface ICurrentUserClaimsAccessor
{
	/// <summary>
	/// Gets the claims.
	/// </summary>
	IReadOnlyCollection<Claim> Claims { get; }
}