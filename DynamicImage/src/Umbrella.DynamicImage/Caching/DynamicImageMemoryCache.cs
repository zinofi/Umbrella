using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.DynamicImage.Caching
{
	public class DynamicImageMemoryCache : DynamicImageCache, IDynamicImageCache
	{
		#region Private Members
		private readonly DynamicImageMemoryCacheOptions _memoryCacheOptions;
		#endregion

		#region Constructors
		public DynamicImageMemoryCache(ILogger<DynamicImageMemoryCache> logger,
			IHybridCache cache,
			ICacheKeyUtility cacheKeyUtility,
			DynamicImageCacheCoreOptions cacheOptions,
			DynamicImageMemoryCacheOptions memoryCacheOptions)
			: base(logger, cache, cacheKeyUtility, cacheOptions)
		{
			_memoryCacheOptions = memoryCacheOptions;
		}
		#endregion

		#region IDynamicImageCache Members
		public Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default)
		{
			try
			{
				string rawKey = GenerateCacheKey(dynamicImage.ImageOptions);
				string cacheKey = GenerateMemoryCacheKey(rawKey);

				Cache.Set(cacheKey, dynamicImage, _memoryCacheOptions);

				return Task.CompletedTask;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
			{
				throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
			}
		}

		public async Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string fileKey = GenerateCacheKey(options);
				string cacheKey = GenerateMemoryCacheKey(fileKey);

				var (itemFound, cacheItem) = Cache.TryGetValue<DynamicImageItem>(cacheKey);

				if (cacheItem != null)
				{
					//If the file does not exist or has been modified since the IDynamicImage was generated,
					//evict it from the cache
					if (sourceLastModified > cacheItem.LastModified)
					{
						await Cache.RemoveAsync<DynamicImageItem>(cacheKey, cancellationToken);

						return null;
					}
				}

				return cacheItem;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { options, sourceLastModified, fileExtension }, returnValue: true))
			{
				throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
			}
		}

		public async Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string key = GenerateCacheKey(options);
				string cacheKey = GenerateMemoryCacheKey(key);

				await Cache.RemoveAsync<DynamicImageItem>(cacheKey, cancellationToken);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { options, fileExtension }, returnValue: true))
			{
				throw new DynamicImageException("There was a problem removing the image from the cache.", exc);
			}
		}
		#endregion

		#region Private Methods
		private string GenerateMemoryCacheKey(string key) => CacheKeyUtility.Create<DynamicImageMemoryCache>(key);
		#endregion
	}
}