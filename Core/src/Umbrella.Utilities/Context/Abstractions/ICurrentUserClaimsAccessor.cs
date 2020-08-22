using System.Collections.Generic;
using System.Security.Claims;

namespace Umbrella.Utilities.Context.Abstractions
{
	/// <summary>
	/// Used to allow access to the claims of the current user.
	/// </summary>
	public interface ICurrentUserClaimsAccessor
	{
		/// <summary>
		/// Gets the claims.
		/// </summary>
		IReadOnlyCollection<Claim> Claims { get; }
	}
}