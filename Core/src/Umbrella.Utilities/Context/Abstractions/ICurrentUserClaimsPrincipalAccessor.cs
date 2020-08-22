using System.Security.Claims;

namespace Umbrella.Utilities.Context.Abstractions
{
	/// <summary>
	/// Used to allow access to the <see cref="ClaimsPrincipal"/> of the current user.
	/// </summary>
	public interface ICurrentUserClaimsPrincipalAccessor
	{
		/// <summary>
		/// Gets the current principal.
		/// </summary>
		ClaimsPrincipal CurrentPrincipal { get; }
	}
}