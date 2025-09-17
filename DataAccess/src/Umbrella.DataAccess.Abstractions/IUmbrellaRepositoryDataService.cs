using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Services.Abstractions;

namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// Defines a generic data service interface for performing CRUD and repository operations on entities using a
/// repository pattern. Supports advanced scenarios with customizable item, entity, repository, and options types.
/// </summary>
/// <remarks>This interface extends IGenericDataService to provide additional flexibility for advanced repository
/// scenarios, allowing customization of repository and entity types as well as repository options. It is intended for
/// use in applications that require fine-grained control over data access patterns and repository
/// configuration.</remarks>
/// <typeparam name="TItem">The type representing the full data transfer object (DTO) for the entity, including all properties required for
/// detailed operations.</typeparam>
/// <typeparam name="TSlimItem">The type representing a lightweight or summary version of the entity, typically used for listing or pagination
/// scenarios.</typeparam>
/// <typeparam name="TPaginatedResultModel">The type representing a paginated result containing a collection of slim items and pagination metadata.</typeparam>
/// <typeparam name="TCreateItem">The type representing the data required to create a new entity.</typeparam>
/// <typeparam name="TCreateResult">The type representing the result returned after creating a new entity.</typeparam>
/// <typeparam name="TUpdateItem">The type representing the data required to update an existing entity.</typeparam>
/// <typeparam name="TUpdateResult">The type representing the result returned after updating an existing entity.</typeparam>
/// <typeparam name="TRepository">The type of the repository used to access and manipulate entities in the data store.</typeparam>
/// <typeparam name="TEntity">The type representing the entity as stored in the data store.</typeparam>
/// <typeparam name="TRepositoryOptions">The type representing repository-specific options or configuration settings.</typeparam>
/// <typeparam name="TEntityKey">The type of the unique key used to identify entities.</typeparam>
public interface IUmbrellaRepositoryDataService<TItem, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult, TRepository, TEntity, TRepositoryOptions, TEntityKey> : IGenericDataService<TItem, TEntityKey, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult>
	where TItem : class, IKeyedItem<TEntityKey>
	where TSlimItem : class, IKeyedItem<TEntityKey>
	where TUpdateItem : class, IKeyedItem<TEntityKey>
	where TEntityKey : IEquatable<TEntityKey>
	where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
	where TRepository : class, IGenericDbRepository<TEntity, TRepositoryOptions, TEntityKey>
	where TEntity : class, IEntity<TEntityKey>
	where TRepositoryOptions : RepoOptions, new();