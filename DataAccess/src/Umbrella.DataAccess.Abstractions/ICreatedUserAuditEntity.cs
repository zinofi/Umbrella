namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing the integer id of the user it was created by.
	/// To specifiy a different user id type, use the generic version of this interface.
	/// </summary>
	public interface ICreatedUserAuditEntity : ICreatedUserAuditEntity<int>
	{
	}

	/// <summary>
	/// Adds support to an entity type for storing the id of the user it was created by.
	/// </summary>
	/// <typeparam name="T">The type of the user id.</typeparam>
	public interface ICreatedUserAuditEntity<T>
	{
		/// <summary>
		/// Gets or sets the id of the user who created this entity.
		/// </summary>
		T CreatedById { get; set; }
	}
}