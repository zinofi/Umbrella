namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing the integer id of the user it was last updated by.
	/// To specifiy a different user id type, use the generic version of this interface.
	/// </summary>
	public interface IUpdatedUserAuditEntity : IUpdatedUserAuditEntity<int>
	{
	}

	/// <summary>
	/// Adds support to an entity type for storing the id of the user it was last updated by.
	/// </summary>
	/// <typeparam name="T">The type of the user id.</typeparam>
	public interface IUpdatedUserAuditEntity<T>
	{
		/// <summary>
		/// Gets or sets the id of the user who last updated this entity.
		/// </summary>
		T UpdatedById { get; set; }
	}
}