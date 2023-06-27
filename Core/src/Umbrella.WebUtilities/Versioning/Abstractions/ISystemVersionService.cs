using Umbrella.WebUtilities.Versioning.Models;

namespace Umbrella.WebUtilities.Versioning.Abstractions;

/// <summary>
/// A service used to obtain version information about the system.
/// </summary>
public interface ISystemVersionService
{
	/// <summary>
	/// Gets the version information.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The version information.</returns>
	ValueTask<SystemVersionModel> GetAsync(CancellationToken cancellationToken = default);
}