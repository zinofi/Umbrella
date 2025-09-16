using Umbrella.Utilities.Data.Concurrency;

namespace Umbrella.Utilities.Data.Models;

/// <summary>
/// A result model of the operation to update an item.
/// </summary>
/// <seealso cref="IConcurrencyStamp" />
public interface IUpdateResultModel : IConcurrencyStamp
{
	// TODO: Consider adding an Id property for convenience.
}