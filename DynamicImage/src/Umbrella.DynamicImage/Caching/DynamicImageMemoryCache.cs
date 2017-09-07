using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using System.Threading.Tasks;
using System.Threading;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageMemoryCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Static Members
        private static readonly Task<DynamicImageItem> s_NullResult = Task.FromResult<DynamicImageItem>(null);
        #endregion

        #region Private Members
        private readonly DynamicImageMemoryCacheOptions m_MemoryCacheOptions;
        #endregion

        #region Constructors
        public DynamicImageMemoryCache(ILogger<DynamicImageMemoryCache> logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageMemoryCacheOptions memoryCacheOptions)
            : base(logger, cache, cacheOptions)
        {
            if (memoryCacheOptions.ItemCacheOptions == null)
                throw new DynamicImageException($"The {memoryCacheOptions.ItemCacheOptions} must not be null");

            m_MemoryCacheOptions = memoryCacheOptions;
        }
        #endregion

        #region IDynamicImageCache Members
        public Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default)
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

        public Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default)
        {
            try
            {
                string key = GenerateCacheKey(options);

                DynamicImageItem item = Cache.Get<DynamicImageItem>(key);

                if (item != null)
                {
                    //If the file does not exist or has been modified since the IDynamicImage was generated,
                    //evict it from the cache
                    if (sourceLastModified > item.LastModified)
                    {
                        Cache.Remove(key);

                        return s_NullResult;
                    }
                }

                return Task.FromResult(item);
            }
            catch(Exception exc) when (Log.WriteError(exc, new { options, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default)
        {
            try
            {
                string key = GenerateCacheKey(options);

                Cache.Remove(key);

                return Task.CompletedTask;
            }
            catch(Exception exc) when (Log.WriteError(exc, new { options, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was a problem removing the image from the cache.", exc);
            }
        }
        #endregion
    }
}