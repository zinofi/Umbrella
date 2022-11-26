using System;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.DataAccess.Remote;

/// <summary>
/// A <see cref="IKeyedItem{TIdentifier}"/> implementation that doesn't do anything. The <see cref="Id"/>
/// property will throw a <see cref="NotImplementedException"/> when accessed.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <seealso cref="IKeyedItem{TIdentifier}" />
public class NoopRemoteItem<TIdentifier> : IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <inheritdoc />
	public TIdentifier Id => throw new NotImplementedException();
}