using System;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	/// <summary>
	/// A remote item that is associated with a specified user.
	/// </summary>
	/// <typeparam name="TUserId">The type of the user identifier.</typeparam>
	public interface IUserScopedRemoteItem<TUserId>
		where TUserId : IEquatable<TUserId>
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		TUserId UserId { get; set; }
	}
}