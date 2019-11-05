using System.Web;
using Umbrella.WebUtilities.Security;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
	public static class HttpContextBaseExtensions
	{
		public static string GetCurrentRequestNonce(this HttpContextBase httpContext) => httpContext.Items[SecurityConstants.DefaultNonceKey] as string;
	}
}