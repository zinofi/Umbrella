namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds support to an entity type for storing it's unique integer Id.
	/// To specify a different Id type, use the generic version of this interface.
	/// </summary>
	public interface IEntity : IEntity<int>
	{
	}

	/// <summary>
	/// Adds support to an entity type for storing it's unique Id.
	/// </summary>
	/// <typeparam name="T">The type of the entity Id.</typeparam>
	public interface IEntity<T>
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		T Id { get; set; }
	}
}