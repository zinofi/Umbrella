using log4net;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.log4net;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
	/// <summary>
	/// Owin Middleware to ensure all environment objects that implement IDisposable are
	/// disposed before a request ends.
	/// </summary>
	public class CleanupIDisposableMiddleware : OwinMiddleware
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(CleanupIDisposableMiddleware));

		public CleanupIDisposableMiddleware(OwinMiddleware next)
			: base(next)
		{
			if (Log.IsDebugEnabled)
				Log.Debug("CleanupIDisposableMiddleware registered successfully");
		}

		public override async Task Invoke(IOwinContext context)
		{
			await Next.Invoke(context);

			try
			{
				//Ensure any disposable objects are disposed correctly before the end of the request
				context.Environment.AsParallel().Select(x => x.Value).OfType<IDisposable>().ForAll(x => x.Dispose());
			}
			catch(Exception exc) when(Log.LogError(exc))
			{
                throw;
			}
		}
	}
}