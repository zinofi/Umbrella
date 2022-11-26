using System.Security.Claims;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context;

/// <summary>
///  A default implementation of the <see cref="ICurrentUserClaimsPrincipalAccessor"/> that essentially does nothing except return a new instance of <see cref="ClaimsPrincipal"/>.
/// </summary>
/// <seealso cref="ICurrentUserClaimsPrincipalAccessor" />
public class DefaultUserClaimsPrincipalAccessor : ICurrentUserClaimsPrincipalAccessor
{
	/// <inheritdoc />
	public ClaimsPrincipal CurrentPrincipal { get; } = new ClaimsPrincipal();
}