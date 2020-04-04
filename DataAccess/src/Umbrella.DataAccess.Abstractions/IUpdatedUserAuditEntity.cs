namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing the id of the user it was last updated by.
	/// </summary>
	/// <typeparam name="TUserId">The type of the user id.</typeparam>
	public interface IUpdatedUserAuditEntity<TUserId>
	{
		/// <summary>
		/// Gets or sets the id of the user who last updated this entity.
		/// </summary>
		TUserId UpdatedById { get; set; }
	}
}