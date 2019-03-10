using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
    /// <summary>
    /// Middleware to log information about the current request for debugging purposes.
    /// If this middleware is included in the OWIN pipeline, it will output debug log messages if that log level
    /// is enabled for the application.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.OwinMiddleware" />
    public class DebugRequestMiddleware : OwinMiddleware
    {
        protected ILogger Log { get; }

        public DebugRequestMiddleware(OwinMiddleware next,
            ILogger<DebugRequestMiddleware> logger)
            : base(next)
        {
            Log = logger;
        }

        public override Task Invoke(IOwinContext context)
        {
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			try
            {
                if(Log.IsEnabled(LogLevel.Debug))
                {
                    var requestData = new
                    {
                        PathBase = context.Request.PathBase.Value,
                        Path = context.Request.Path.Value,
                        context.Request.Headers
                    };

                    Log.WriteDebug(requestData, "Debug request state");
                }

                return Next.Invoke(context);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
    }
}