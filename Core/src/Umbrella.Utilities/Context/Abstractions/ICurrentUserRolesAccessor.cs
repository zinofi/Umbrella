using System.Collections.Generic;

namespace Umbrella.Utilities.Context.Abstractions
{
	/// <summary>
	/// Used to allow access to the roles of the current user.
	/// </summary>
	/// <typeparam name="T">The type of the role.</typeparam>
	public interface ICurrentUserRolesAccessor<T>
	{
		/// <summary>
		/// Gets the role names.
		/// </summary>
		IReadOnlyCollection<string> RoleNames { get; }

		/// <summary>
		/// Gets the roles.
		/// </summary>
		IReadOnlyCollection<T> Roles { get; }
	}
}