using System;
using System.Collections.Generic;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context
{
	public class DefaultUserRolesAccessor<T> : ICurrentUserRolesAccessor<T>
	{
		public IReadOnlyCollection<string> RoleNames { get; } = Array.Empty<string>();
		public IReadOnlyCollection<T> Roles { get; } = Array.Empty<T>();
	}
}