using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Cookie.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Cookie;

/// <summary>
/// A service used to store and retrieve JSON serializable objects from cookies.
/// </summary>
internal sealed class JsonCookieService : IJsonCookieService
{
	private readonly ILogger _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public JsonCookieService(
		ILogger<JsonCookieService> logger,
		IHttpContextAccessor httpContextAccessor)
	{
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
	}

	/// <inheritdoc />
	public void Set<T>(T value, TimeSpan? expiration = null, bool httpOnly = true)
	{
		Type cookieType = typeof(T);

		try
		{
			var options = new CookieOptions()
			{
				Expires = expiration.HasValue ? DateTime.Now.Add(expiration.Value) : DateTime.MaxValue,
				HttpOnly = httpOnly
			};

			string strValue = JsonSerializer.Serialize(value);

			_httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieType.Name, strValue, options);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { cookieType, value, expiration }))
		{
			throw new InvalidOperationException("There has been an error while trying to set the cookie value", exc);
		}
	}

	/// <inheritdoc />
	public T? Get<T>(bool throwOnError = false)
	{
		Type cookieType = typeof(T);

		try
		{
			string? cookie = _httpContextAccessor.HttpContext?.Request.Cookies[cookieType.Name];

			return string.IsNullOrEmpty(cookie)
				? default
				: JsonSerializer.Deserialize<T>(cookie);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { cookieType, throwOnError }))
		{
			if (throwOnError)
				throw new InvalidOperationException("There has been an error while trying to get the cookie value", exc);

			return default;
		}
	}
}