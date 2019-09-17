using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.Utilities.Hosting.Options;
using Umbrella.Utilities.Primitives;

namespace Umbrella.Utilities.Hosting
{
	public abstract class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
	{
		#region Protected Properties
		protected ILogger Log { get; }
		protected UmbrellaHostingEnvironmentOptions Options { get; }
		protected IMemoryCache Cache { get; }
		protected ICacheKeyUtility CacheKeyUtility { get; }

		// Exposed as internal for unit testing / benchmarking mocks
		protected internal IFileProvider ContentRootFileProvider { get; set; }

		// Exposed as internal for unit testing / benchmarking mocks
		protected internal IFileProvider WebRootFileProvider { get; set; }
		#endregion

		#region Constructors
		public UmbrellaHostingEnvironment(
			ILogger logger,
			UmbrellaHostingEnvironmentOptions options,
			IMemoryCache cache,
			ICacheKeyUtility cacheKeyUtility)
		{
			Log = logger;
			Options = options;
			Cache = cache;
			CacheKeyUtility = cacheKeyUtility;
		}
		#endregion

		#region IUmbrellaHostingEnvironment Members
		public abstract string MapPath(string virtualPath, bool fromContentRoot = true);

		public virtual async Task<string> GetFileContentAsync(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true)
		{
			Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

			string[] cacheKeyParts = null;

			try
			{
				cacheKeyParts = ArrayPool<string>.Shared.Rent(4);
				cacheKeyParts[0] = virtualPath;
				cacheKeyParts[1] = fromContentRoot.ToString();
				cacheKeyParts[2] = cache.ToString();
				cacheKeyParts[3] = watch.ToString();

				string key = CacheKeyUtility.Create<UmbrellaHostingEnvironment>(cacheKeyParts, 4);

				string physicalPath = MapPath(virtualPath, fromContentRoot);

				async Task<string> ReadContent()
				{
					IFileProvider fileProvider = fromContentRoot ? ContentRootFileProvider : WebRootFileProvider;
					IFileInfo fileInfo = fileProvider.GetFileInfo(physicalPath);

					if (fileInfo.Exists)
					{
						using (Stream fs = fileInfo.CreateReadStream())
						{
							using (var sr = new StreamReader(fs))
							{
								// This is all going to end up in the cache anyway so we can live with sync here.
								return await sr.ReadToEndAsync().ConfigureAwait(false);
							}
						}
					}

					return null;
				}

				if (cache)
				{
					return await Cache.GetOrCreateAsync(key, async entry =>
					{
						entry.SetSlidingExpiration(TimeSpan.FromHours(1));

						if (watch)
							entry.AddExpirationToken(new PhysicalFileChangeToken(new FileInfo(physicalPath)));

						return await ReadContent().ConfigureAwait(false);
					});
				}
				else
				{
					return await ReadContent().ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot, cache, watch }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem reading the contents of the specified file.", exc);
			}
			finally
			{
				if (cacheKeyParts != null)
					ArrayPool<string>.Shared.Return(cacheKeyParts);
			}
		}
		#endregion
	}
}