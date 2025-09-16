using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Concurrency;

namespace Umbrella.Utilities.Data.Models;

/// <summary>
/// A model that is used to update an item.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IUpdateModel<TKey> : IConcurrencyStamp, IKeyedItem<TKey>
	where TKey : IEquatable<TKey>
{
}