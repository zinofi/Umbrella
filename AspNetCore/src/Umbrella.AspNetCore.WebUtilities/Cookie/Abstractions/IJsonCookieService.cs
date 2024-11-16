namespace Umbrella.AspNetCore.WebUtilities.Cookie.Abstractions;

/// <summary>
/// A service used to store and retrieve JSON serializable objects from cookies.
/// </summary>
public interface IJsonCookieService
{
	/// <summary>
	/// Gets the value of the cookie as the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the object to retrieve.</typeparam>
	/// <param name="throwOnError">Specifies whether an exception should be thrown if an error occurs.</param>
	/// <returns>The object of the specified type if it exists; otherwise, <c>null</c>.</returns>
	T? Get<T>(bool throwOnError = false);

	/// <summary>
	/// Sets the value of the cookie.
	/// </summary>
	/// <typeparam name="T">The type of the object to store.</typeparam>
	/// <param name="value">The object to store.</param>
	/// <param name="expiration">The optional expiration time for the cookie.</param>
	/// <param name="httpOnly">Specifies whether the cookie should be accessible only through HTTP.</param>
	void Set<T>(T value, TimeSpan? expiration = null, bool httpOnly = true);
}