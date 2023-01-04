using System.Security.Claims;

namespace Umbrella.AspNetCore.Blazor.Security.Abstractions;

/// <summary>
/// An authentication state provider used to mark a <see cref="ClaimsPrincipal"/> as being authenticated as the current user, or mark any existing one
/// as logged out.
/// </summary>
public interface IClaimsPrincipalAuthenticationStateProvider
{
	/// <summary>
	/// Occurs when the authenticate state of the current <see cref="ClaimsPrincipal"/> has changed.
	/// </summary>
	event EventHandler? AuthenticatedStateHasChanged;

	/// <summary>
	/// Marks the user as authenticated.
	/// </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>A task that completes when the operation has completed.</returns>
	Task MarkUserAsAuthenticatedAsync(ClaimsPrincipal principal);

	/// <summary>
	/// Marks the currrent <see cref="ClaimsPrincipal"/> as logged out.
	/// </summary>
	/// <returns>A task that completes when the operation has completed.</returns>
	Task MarkUserAsLoggedOutAsync();
}