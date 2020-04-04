namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing it's unique Id.
	/// </summary>
	/// <typeparam name="TEntityKey">The type of the entity Id.</typeparam>
	public interface IEntity<TEntityKey>
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		TEntityKey Id { get; set; }
	}
}