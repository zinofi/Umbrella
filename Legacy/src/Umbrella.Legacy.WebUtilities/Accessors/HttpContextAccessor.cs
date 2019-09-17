using System.Threading;
using System.Web;
using Umbrella.Legacy.WebUtilities.Accessors.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Accessors
{
	// TODO v3.x
	// This code has been copied from the ASP.NET Core repo.
	// Not sure if this will work but going to give it a go.
	// Only works in conjunction with the HttpContextAccessorMiddleware.
	internal class HttpContextAccessor : IHttpContextAccessor
	{
		private class HttpContextHolder
		{
			public HttpContext Context;
		}

		private static readonly AsyncLocal<HttpContextHolder> _httpContextCurrent = new AsyncLocal<HttpContextHolder>();

		public HttpContext HttpContext
		{
			get => _httpContextCurrent.Value?.Context;
			set
			{
				HttpContextHolder holder = _httpContextCurrent.Value;

				if (holder != null)
				{
					// Clear current HttpContext trapped in the AsyncLocals, as its done.
					holder.Context = null;
				}

				if (value != null)
				{
					// Use an object indirection to hold the HttpContext in the AsyncLocal,
					// so it can be cleared in all ExecutionContexts when its cleared.
					_httpContextCurrent.Value = new HttpContextHolder { Context = value };
				}
			}
		}
	}
}