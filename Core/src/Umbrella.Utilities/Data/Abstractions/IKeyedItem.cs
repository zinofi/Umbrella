using System;

namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An item that has a key of type <typeparamref name="TIdentifier"/>.
	/// </summary>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	public interface IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		TIdentifier Id { get; }
	}
}