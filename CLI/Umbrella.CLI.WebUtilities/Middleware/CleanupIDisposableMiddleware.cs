using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Extensions;

namespace Umbrella.CLI.WebUtilities.Middleware
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
			await m_Next.Invoke(context);

			try
			{
				//Ensure any disposable objects are disposed correctly before the end of the request
                context.Response.OnCompleted(() => new Task(() =>
                {
                    context.Items.AsParallel().Select(x => x.Value).OfType<IDisposable>().ForAll(x => x.Dispose());
                }));
            }
			catch(Exception exc) when (m_Logger.LogError(exc, returnValue: DebugUtility.IsDebugMode))
			{
                throw;
			}
		}
        #endregion
    }
}