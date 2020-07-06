using System;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	/// <summary>
	/// A generic HTTP Service that forms part of a collection of services that each have the same <typeparamref name="TItem"/> and <typeparamref name="TIdentifier"/> but whose
	/// <typeparamref name="TRemoteSource"/> differ.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TRemoteSource">The type of the remote source type.</typeparam>
	/// <seealso cref="Umbrella.DataAccess.Remote.Abstractions.IGenericHttpService{TItem, TIdentifier}" />
	public interface IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource> : IGenericHttpService<TItem, TIdentifier>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
	{
		/// <summary>
		/// Gets the type of the remote source.
		/// </summary>
		TRemoteSource RemoteSourceType { get; }
	}
}