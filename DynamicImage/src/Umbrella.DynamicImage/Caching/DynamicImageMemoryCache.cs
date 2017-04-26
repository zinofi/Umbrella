using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Umbrella.Utilities.Mime;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageMemoryCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Members
        private readonly DynamicImageMemoryCacheOptions m_MemoryCacheOptions;
        private readonly IUmbrellaHostingEnvironment m_UmbrellaHostingEnvironment;
        #endregion

        #region Constructors
        public DynamicImageMemoryCache(ILogger<DynamicImageMemoryCache> logger,
            IMimeTypeUtility mimeTypeUtility,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageMemoryCacheOptions memoryCacheOptions,
            IUmbrellaHostingEnvironment umbrellaHostingEnvironment)
            : base(logger, mimeTypeUtility, cache, cacheOptions)
        {
            if (memoryCacheOptions.ItemCacheOptions == null)
                throw new DynamicImageException($"The {memoryCacheOptions.ItemCacheOptions} must not be null");

            m_MemoryCacheOptions = memoryCacheOptions;
            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
        }
        #endregion

        #region IDynamicImageCache Members
        public Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                Cache.GetOrCreate(key, entry =>
                {
                    entry.SetOptions(m_MemoryCacheOptions.ItemCacheOptions);

                    return dynamicImage;
                });

                return Task.CompletedTask;
            }
            catch(Exception exc) when (Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public Task<DynamicImageItem> GetAsync(string key, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                DynamicImageItem item = Cache.Get<DynamicImageItem>(key);

                if (item != null)
                {
                    //If the file does not exist or has been modified since the IDynamicImage was generated,
                    //evict it from the cache
                    if (sourceLastModified > item.LastModified)
                    {
                        Cache.Remove(key);

                        return null;
                    }
                }

                return Task.FromResult(item);
            }
            catch(Exception exc) when (Log.WriteError(exc, new { key, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public Task RemoveAsync(string key, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Cache.Remove(key);

                return Task.CompletedTask;
            }
            catch(Exception exc) when (Log.WriteError(exc, new { key, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was a problem removing the image from the cache.", exc);
            }
        }
        #endregion
    }
}