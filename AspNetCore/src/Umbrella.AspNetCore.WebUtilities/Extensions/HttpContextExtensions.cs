using Microsoft.AspNetCore.Http;
using Umbrella.WebUtilities.Security;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="HttpContext"/> class.
	/// </summary>
	public static class HttpContextExtensions
	{
		/// <summary>
		/// Gets the value of the <see cref="NonceContext.Current"/> property stored in the <see cref="NonceContext"/> in the <see cref="HttpContext.Features"/> collection.
		/// This needs to be manually added first as part of the current request, typically using middleware.
		/// </summary>
		/// <param name="httpContext">The current HTTP context.</param>
		/// <returns>The value of <see cref="NonceContext.Current"/> stored on the current HTTP context.</returns>
		public static string GetCurrentRequestNonce(this HttpContext httpContext) => httpContext.Features.Get<NonceContext>()?.Current;
	}
}