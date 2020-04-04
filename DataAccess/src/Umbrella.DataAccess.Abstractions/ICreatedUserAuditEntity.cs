namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing the id of the user it was created by.
	/// </summary>
	/// <typeparam name="TUserId">The type of the user id.</typeparam>
	public interface ICreatedUserAuditEntity<TUserId>
	{
		/// <summary>
		/// Gets or sets the id of the user who created this entity.
		/// </summary>
		TUserId CreatedById { get; set; }
	}
}