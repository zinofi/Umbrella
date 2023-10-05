namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// A repository used to access database version information.
/// </summary>
public interface IDatabaseVersionRepository
{
	/// <summary>
	/// Gets the current database version.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The current database version.</returns>
	Task<string?> GetDatabaseVersionAsync(CancellationToken cancellationToken = default);
}