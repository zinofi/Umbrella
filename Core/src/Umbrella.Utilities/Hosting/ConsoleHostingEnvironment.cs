using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Hosting
{
    public class ConsoleHostingEnvironment : UmbrellaHostingEnvironment
	{
		public ConsoleHostingEnvironment(
			ILogger<ConsoleHostingEnvironment> logger,
			IMemoryCache cache,
			ICacheKeyUtility cacheKeyUtility)
			: base(logger, cache, cacheKeyUtility)
		{
		}

		public override string MapPath(string virtualPath, bool fromContentRoot = true)
		{
			Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

			if (!fromContentRoot)
				throw new ArgumentException("This value must always be true in a classic .NET application. It can only be set to false inside a .NET Core application.", nameof(fromContentRoot));

			try
			{
				string key = CacheKeyUtility.Create<ConsoleHostingEnvironment>(new string[] { virtualPath, fromContentRoot.ToString() });

				return Cache.GetOrCreate(key, entry =>
				{
					entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

					string cleanedPath = virtualPath.Replace("~/", "").Replace('/', Path.DirectorySeparatorChar);

					// TODO: Need to add Unit Tests.
					return Path.Combine(AppContext.BaseDirectory, cleanedPath);
				});
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot }))
			{
				throw new UmbrellaException("There was a problem mapping the path.", exc);
			}
		}
	}
}
