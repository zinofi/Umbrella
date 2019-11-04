using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.Utilities.Hosting.Options;

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
		protected internal Lazy<IFileProvider> ContentRootFileProvider { get; set; }

		// TODO: This shouldn't be on here. 
		// Exposed as internal for unit testing / benchmarking mocks
		protected internal Lazy<IFileProvider> WebRootFileProvider { get; set; }
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

		public virtual async Task<string> GetFileContentAsync(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
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

				string cleanedPath = TransformPathForFileProvider(virtualPath);

				async Task<(string content, IChangeToken changeToken)> ReadContentAsync()
				{
					IFileProvider fileProvider = fromContentRoot ? ContentRootFileProvider.Value : WebRootFileProvider.Value;
					IFileInfo fileInfo = fileProvider.GetFileInfo(cleanedPath);

					if (fileInfo.Exists)
					{
						using Stream fs = fileInfo.CreateReadStream();
						using var sr = new StreamReader(fs);

						string content = await sr.ReadToEndAsync().ConfigureAwait(false);

						return watch ? (content, fileProvider.Watch(cleanedPath)) : (content, null);
					}

					return default;
				}

				if (cache)
				{
					return await Cache.GetOrCreateAsync(key, async entry =>
					{
						entry.SetSlidingExpiration(Options.CacheTimeout);

						var (content, changeToken) = await ReadContentAsync().ConfigureAwait(false);

						if (watch && changeToken != null)
							entry.AddExpirationToken(changeToken);

						return content;
					});
				}
				else
				{
					var (content, changeToken) = await ReadContentAsync().ConfigureAwait(false);

					return content;
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

		#region Protected Methods		
		/// <summary>
		/// Transforms the path for use with an <see cref="IFileProvider"/>.
		/// </summary>
		/// <param name="virtualPath">The virtual path.</param>
		/// <returns>The transformed path.</returns>
		protected abstract string TransformPathForFileProvider(string virtualPath);
		#endregion
	}
}