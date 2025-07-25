namespace Umbrella.Utilities.Data.Abstractions;

/// <summary>
/// A <see cref="IKeyedItem{TIdentifier}"/> implementation that doesn't do anything. The <see cref="Id"/>
/// property will throw a <see cref="NotImplementedException"/> when accessed.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
/// <seealso cref="IKeyedItem{TIdentifier}" />
public class NoOpKeyedItem<TIdentifier> : IKeyedItem<TIdentifier>
	where TIdentifier : IEquatable<TIdentifier>
{
	/// <inheritdoc />
	public TIdentifier Id => throw new NotImplementedException();
}