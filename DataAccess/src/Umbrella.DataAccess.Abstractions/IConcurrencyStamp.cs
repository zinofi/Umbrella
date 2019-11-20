namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Adds supports for storing a concurrency token on an entity type.
	/// </summary>
	public interface IConcurrencyStamp
	{
		/// <summary>
		/// Gets or sets the concurrency stamp.
		/// </summary>
		string ConcurrencyStamp { get; set; }
	}
}