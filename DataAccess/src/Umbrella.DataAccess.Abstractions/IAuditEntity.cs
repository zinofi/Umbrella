namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing it's unique integer Id, the integer id of the user it was both created by and last updated by, toegther with
	/// support for storing the date it was created and last updated.
	/// To specifiy a different id type, use the generic version of this interface.
	/// </summary>
	public interface IAuditEntity : IAuditEntity<int>
	{
	}

	/// <summary>
	/// Adds support to an entity type for storing it's unique Id, the id of the user it was both created by and last updated by, toegther with
	/// support for storing the date it was created and last updated.
	/// This is a convenience interface that implements <see cref="IEntity{T}"/>, <see cref="ICreatedDateAuditEntity"/>, <see cref="ICreatedUserAuditEntity{T}"/>, <see cref="IUpdatedDateAuditEntity"/> and <see cref="IUpdatedUserAuditEntity{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the user id.</typeparam>
	public interface IAuditEntity<T> : IEntity<T>, ICreatedDateAuditEntity, ICreatedUserAuditEntity<T>, IUpdatedDateAuditEntity, IUpdatedUserAuditEntity<T>
	{
	}
}