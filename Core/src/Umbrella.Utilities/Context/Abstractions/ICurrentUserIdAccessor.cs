namespace Umbrella.Utilities.Context.Abstractions;

/// <summary>
/// Used to allow access to the id of the current user.
/// </summary>
/// <typeparam name="T">The type of the user id.</typeparam>
[Obsolete("Please use ClaimsPrincipal.Current to access. Call the GetId<T> extension method.", true)]
public interface ICurrentUserIdAccessor<T>
{
	/// <summary>
	/// Gets the current user identifier.
	/// </summary>
	T CurrentUserId { get; }
}