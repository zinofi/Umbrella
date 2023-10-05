using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context;

/// <summary>
/// A default implementation of the <see cref="ICurrentUserRolesAccessor{T}"/> that essentially does nothing except return empty arrays for all properties.
/// </summary>
/// <typeparam name="TRole">The type of the role.</typeparam>
public sealed class DefaultUserRolesAccessor<TRole> : ICurrentUserRolesAccessor<TRole>
	where TRole : struct, Enum
{
	/// <inheritdoc />
	public IReadOnlyCollection<string> RoleNames { get; } = Array.Empty<string>();

	/// <inheritdoc />
	public IReadOnlyCollection<TRole> Roles { get; } = Array.Empty<TRole>();
}