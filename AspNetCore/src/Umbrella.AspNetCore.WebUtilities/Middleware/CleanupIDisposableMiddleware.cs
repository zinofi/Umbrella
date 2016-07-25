using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.WebUtilities.Middleware
{
	/// <summary>
	/// ASP.NET Middleware to ensure all HttpContext objects that implement IDisposable are
	/// disposed before a request ends.
	/// </summary>
	public class CleanupIDisposableMiddleware
	{
        #region Private Members
        private readonly RequestDelegate m_Next;
        private readonly ILogger<CleanupIDisposableMiddleware> m_Logger;
        #endregion

        #region Constructors
        public CleanupIDisposableMiddleware(RequestDelegate next, ILogger<CleanupIDisposableMiddleware> logger)
		{
            m_Next = next;
            m_Logger = logger;
		}
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
		{
			try
			{
				//Ensure any disposable objects are disposed correctly before the end of the request
                context.Response.OnCompleted(() => new Task(() =>
                {
                    context.Items.AsParallel().Select(x => x.Value).OfType<IDisposable>().ForAll(x => x.Dispose());
                }));
            }
			catch(Exception exc) when (m_Logger.WriteError(exc, returnValue: !DebugUtility.IsDebugMode))
			{
                //Should enter here when in DebugMode. It's not critical if something goes wrong during resource disposal as the GC will
                //sort us out eventually unless we've created something that can't be cleaned up automatically.
			}
            finally
            {
                await m_Next.Invoke(context);
            }
		}
        #endregion
    }
}