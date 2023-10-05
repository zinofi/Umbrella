namespace Umbrella.WebUtilities.Middleware.Options;

/// <summary>
/// Specifies the HttpCacheControl values that can be used with middleware.
/// </summary>
public enum MiddlewareHttpCacheability
{
	/// <summary>
	/// Indicates that the response may be cached by any cache, even if the response would normally be non-cacheable (e.g. if the response does not contain a max-age directive or the Expires header).
	/// </summary>
	Public,

	/// <summary>
	/// Indicates that the response is intended for a single user and must not be stored by a shared cache. A private cache may store the response.
	/// </summary>
	Private,

	/// <summary>
	/// Forces caches to submit the request to the origin server for validation before releasing a cached copy.
	/// </summary>
	NoCache,

	/// <summary>
	/// The cache should not store anything about the client request or server response.
	/// </summary>
	NoStore
}

/// <summary>
/// Extension methods for the <see cref="MiddlewareHttpCacheability"/> enum type.
/// </summary>
public static class MiddlewareHttpCacheabilityExtensions
{
	/// <summary>
	/// Converts to the enum value to its corresponding Cache-Control header value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The HTTP Cache-Control header value.</returns>
	/// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> has an unknown value.</exception>
	public static string ToCacheControlString(this MiddlewareHttpCacheability value) => value switch
	{
		MiddlewareHttpCacheability.NoCache => "no-cache",
		MiddlewareHttpCacheability.NoStore => "no-store",
		MiddlewareHttpCacheability.Private => "private",
		MiddlewareHttpCacheability.Public => "public",
		_ => throw new ArgumentException($"The specified value: {value} is not supported.", nameof(value))
	};
}