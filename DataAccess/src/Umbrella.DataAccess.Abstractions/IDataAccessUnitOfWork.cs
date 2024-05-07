namespace Umbrella.DataAccess.Abstractions;

public interface IDataAccessUnitOfWork
{
	/// <summary>
	/// Commits the changes to the underlying data store.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable task.</returns>
	Task CommitAsync(CancellationToken cancellationToken = default);
}