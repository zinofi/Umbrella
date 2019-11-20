namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Used to allow access to the integer id of the current user.
	/// To specifiy a different user id type, use the generic version of this interface.
	/// </summary>
	public interface ICurrentUserIdAccessor : ICurrentUserIdAccessor<int>
	{
	}

	/// <summary>
	/// Used to allow access to the id of the current user.
	/// </summary>
	/// <typeparam name="T">The type of the user id.</typeparam>
	public interface ICurrentUserIdAccessor<T>
	{
		/// <summary>
		/// Gets the current user identifier.
		/// </summary>
		T CurrentUserId { get; }
	}
}