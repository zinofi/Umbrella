using log4net;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DNX.WebUtilities.Middleware
{
	/// <summary>
	/// ASP.NET Middleware to ensure all HttpContext objects that implement IDisposable are
	/// disposed before a request ends.
	/// </summary>
	public class CleanupIDisposableMiddleware
	{
        #region Private Static Members
        private static readonly ILog Log = LogManager.GetLogger(typeof(CleanupIDisposableMiddleware));
        #endregion

        #region Private Members
        private readonly RequestDelegate m_Next;
        #endregion

        #region Constructors
        public CleanupIDisposableMiddleware(RequestDelegate next)
		{
            m_Next = next;
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
			catch(Exception exc)
			{
				Log.LogError(exc);
			}
		}
        #endregion
    }
}