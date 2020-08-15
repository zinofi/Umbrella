using System;
using Umbrella.DataAccess.Remote.Abstractions;

namespace Umbrella.DataAccess.Remote
{
	/// <summary>
	/// A <see cref="IRemoteItem{TIdentifier}"/> implementation that doesn't do anything. The <see cref="Id"/>
	/// property will throw a <see cref="NotImplementedException"/> when accessed.
	/// </summary>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <seealso cref="Umbrella.DataAccess.Remote.Abstractions.IRemoteItem{TIdentifier}" />
	public class NoopRemoteItem<TIdentifier> : IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		/// <inheritdoc />
		public TIdentifier Id => throw new NotImplementedException();
	}
}