using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbrella.Legacy.WebUtilities.Accessors.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
    internal class HttpContextAccessorMiddleware : OwinMiddleware
    {
        protected ILogger Log { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }

        public HttpContextAccessorMiddleware(
            OwinMiddleware next,
            ILogger<HttpContextAccessorMiddleware> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(next)
        {
            Log = logger;
            HttpContextAccessor = httpContextAccessor;
        }

        public override async Task Invoke(IOwinContext context)
        {
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			try
            {
                // Assign the context at the start of the request.
                HttpContextAccessor.HttpContext = HttpContext.Current;

                await Next.Invoke(context);
				context.Request.CallCancelled.ThrowIfCancellationRequested();

				// Clear the context at the end of the request.
				HttpContextAccessor.HttpContext = null;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
			finally
			{
				// TODO: Should the clearing be done here.
			}
        }
    }
}