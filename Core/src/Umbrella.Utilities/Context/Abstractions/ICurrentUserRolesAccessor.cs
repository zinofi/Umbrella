using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Context.Abstractions
{
	/// <summary>
	/// Used to allow access to the roles of the current user.
	/// </summary>
	/// <typeparam name="TRole">The type of the role.</typeparam>
	public interface ICurrentUserRolesAccessor<TRole>
		where TRole : struct, Enum
	{
		/// <summary>
		/// Gets the role names.
		/// </summary>
		IReadOnlyCollection<string> RoleNames { get; }

		/// <summary>
		/// Gets the roles.
		/// </summary>
		IReadOnlyCollection<TRole> Roles { get; }
	}
}