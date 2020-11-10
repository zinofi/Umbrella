using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Encapsulates CRUD operations for a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <seealso cref="F:Umbrella.DataAccess.Abstractions.IGenericDbRepository{TEntity, Umbrella.DataAccess.Abstractions.RepoOptions}" />
	public interface IGenericDbRepository<TEntity> : IGenericDbRepository<TEntity, RepoOptions>
		where TEntity : class, IEntity<int>
	{
	}

	/// <summary>
	/// Encapsulates CRUD operations for a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <seealso cref="F:Umbrella.DataAccess.Abstractions.IGenericDbRepository{TEntity, Umbrella.DataAccess.Abstractions.RepoOptions}" />
	public interface IGenericDbRepository<TEntity, in TRepoOptions> : IGenericDbRepository<TEntity, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TRepoOptions : RepoOptions, new()
	{
	}

	/// <summary>
	/// Encapsulates CRUD operations for a database repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TRepoOptions">The type of the repo options.</typeparam>
	/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
	/// <seealso cref="F:Umbrella.DataAccess.Abstractions.IGenericDbRepository{TEntity, Umbrella.DataAccess.Abstractions.RepoOptions}" />
	public interface IGenericDbRepository<TEntity, in TRepoOptions, TEntityKey> : IReadOnlyGenericDbRepository<TEntity, TRepoOptions, TEntityKey>
		where TEntity : class, IEntity<TEntityKey>
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		/// <summary>
		/// Removes the empty entities from the <paramref name="entities"/> collection. This collection will be mutated by this method.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		Task RemoveEmptyEntitiesAsync(ICollection<TEntity> entities, CancellationToken cancellationToken, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);

		/// <summary>
		/// Saves the entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="pushChangesToDb">if set to <c>true</c> pushes the changes to the database.</param>
		/// <param name="addToContext">if set to <c>true</c> adds the entity to the database context.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <param name="forceAdd">if set to <c>true</c> forces the entity to be added to the database. This is useful when the repository cannot determine if the item is new or not.</param>
		/// <returns>The result of the save operation.</returns>
		Task<SaveResult<TEntity>> SaveAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool addToContext = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null, bool forceAdd = false);

		/// <summary>
		/// Saves all entities in the specified collection.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="pushChangesToDb">if set to <c>true</c> pushes the changes to the database.</param>
		/// <param name="bypassSaveLogic">if set to <c>true</c> bypasses the default save logic.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>The result of the save operation.</returns>
		Task<IReadOnlyCollection<SaveResult<TEntity>>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, bool bypassSaveLogic = false, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);

		/// <summary>
		/// Deletes the entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="pushChangesToDb">if set to <c>true</c> pushes the changes to the database.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);

		/// <summary>
		/// Deletes all of the entities in the specified collection.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="pushChangesToDb">if set to <c>true</c> pushes the changes to the database.</param>
		/// <param name="repoOptions">The repo options.</param>
		/// <param name="childOptions">The child options.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool pushChangesToDb = true, TRepoOptions repoOptions = null, IEnumerable<RepoOptions> childOptions = null);
	}
}