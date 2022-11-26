using System.Web;
using Umbrella.WebUtilities.Security;

namespace Umbrella.Legacy.WebUtilities.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="HttpContextBase"/> type.
/// </summary>
public static class HttpContextBaseExtensions
{
	/// <summary>
	/// Gets the current request nonce.
	/// </summary>
	/// <param name="httpContext">The HTTP context.</param>
	/// <returns>The nonce value for the current request.</returns>
	public static string? GetCurrentRequestNonce(this HttpContextBase httpContext) => httpContext.Items[SecurityConstants.DefaultNonceKey] as string;
}