namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// A utility used by derived DbContext types to coordinate code execution with repositorites.
/// The lifetime of this utility is registered with the DI container as Scoped to tie it to the lifetime of the DbContext.
/// </summary>
public interface IUmbrellaDbContextHelper
{
	/// <summary>
	/// Executes the pending actions.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A awaitable <see cref="Task"/> which will complete when all pending actions have been executed.</returns>
	Task ExecutePostSaveChangesActionsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Registers an asynchronous action to be executed after pending changes on a DbContext have been persisted.
	/// </summary>
	/// <param name="entity">The entity.</param>
	/// <param name="wrappedAction">The wrapped action.</param>
	void RegisterPostSaveChangesAction(object entity, Func<CancellationToken, Task> wrappedAction);

	/// <summary>
	/// Saves the changes.
	/// </summary>
	/// <param name="baseSaveChanges">The SaveChanges method on the DbContext should be passed in.</param>
	/// <returns>The value returned from the call made internally to the DbContext.SaveChanges method.</returns>
	int SaveChanges(Func<int> baseSaveChanges);

	/// <summary>
	/// Saves the changes.
	/// </summary>
	/// <param name="baseSaveChangesAsync">The SaveChangesAsync method on the DbContext should be passed in.</param>
	/// <param name="cancellationToken">The </param>
	/// <returns>The value returned from the call made internally to the DbContext.SaveChanges method.</returns>
	Task<int> SaveChangesAsync(Func<CancellationToken, Task<int>> baseSaveChangesAsync, CancellationToken cancellationToken = default);
}