using System;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	/// <summary>
	/// An item that can be located in multiple locations.
	/// </summary>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TRemoteSource">The type of the remote source type.</typeparam>
	/// <seealso cref="Umbrella.DataAccess.Remote.Abstractions.IRemoteItem{TIdentifier}" />
	public interface IMultiRemoteItem<TIdentifier, TRemoteSource> : IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
	{
		/// <summary>
		/// Gets or sets the source of the item.
		/// </summary>
		TRemoteSource Source { get; set; }
	}
}