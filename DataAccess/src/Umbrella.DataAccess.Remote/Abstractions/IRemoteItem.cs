using System;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	/// <summary>
	/// An item that can be loaded from a remote location.
	/// </summary>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	public interface IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		TIdentifier Id { get; }
	}
}