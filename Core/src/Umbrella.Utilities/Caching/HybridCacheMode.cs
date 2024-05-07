using System.Diagnostics.CodeAnalysis;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.Utilities.Caching;

/// <summary>
/// The caching mode for the <see cref="IHybridCache"/>.
/// </summary>
[SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "This enum does not support flags.")]
public enum HybridCacheMode
{
	/// <summary>
	/// Cache items will be stored in memory.
	/// </summary>
	Memory = 0,

	///// <summary>
	///// Cache items will be stored in memory. However, when adding 2 items that have the same key,
	///// if the item to be added needs to be built using a provided delegate, that delegate will be guaranteed
	///// to only be executed once in the event of a race condition.
	///// </summary>
	// TODO: MemoryMutex = 1,

	/// <summary>
	/// Cache items will be stored in the distributed cache configured for the application.
	/// </summary>
	Distributed = 2,

	///// <summary>
	///// Cache items will be stored in the distributed cache configured for the application.
	///// However, when adding 2 items that have the same key,
	///// if the item to be added needs to be built using a provided delegate, that delegate will be guaranteed
	///// to only be executed once in the event of a race condition.
	///// </summary>
	// TODO: DistributedMutex = 3
}