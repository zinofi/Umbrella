namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// Represents a unit of work for data access operations.
/// </summary>
public interface IDataAccessUnitOfWork
{
	/// <summary>
	/// Commits the changes to the underlying data store.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable task.</returns>
	Task CommitAsync(CancellationToken cancellationToken = default);
}