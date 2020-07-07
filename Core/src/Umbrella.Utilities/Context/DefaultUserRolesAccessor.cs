using System;
using System.Collections.Generic;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context
{
	/// <summary>
	/// A default implementation of the <see cref="ICurrentUserRolesAccessor{T}"/> that essentially does nothing except return empty arrays for all properties.
	/// </summary>
	/// <typeparam name="T">The type of the role.</typeparam>
	public sealed class DefaultUserRolesAccessor<T> : ICurrentUserRolesAccessor<T>
	{
		/// <inheritdoc />
		public IReadOnlyCollection<string> RoleNames { get; } = Array.Empty<string>();

		/// <inheritdoc />
		public IReadOnlyCollection<T> Roles { get; } = Array.Empty<T>();
	}
}