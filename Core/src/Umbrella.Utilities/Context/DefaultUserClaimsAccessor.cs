using System.Security.Claims;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context;

/// <summary>
/// A default implementation of the <see cref="ICurrentUserClaimsAccessor"/> that essentially does nothing except return empty arrays for all properties.
/// </summary>
public class DefaultUserClaimsAccessor : ICurrentUserClaimsAccessor
{
	/// <inheritdoc />
	public IReadOnlyCollection<Claim> Claims => throw new NotImplementedException();
}