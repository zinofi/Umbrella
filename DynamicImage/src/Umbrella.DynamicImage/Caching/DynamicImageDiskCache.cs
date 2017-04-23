using Umbrella.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using System.Threading;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageDiskCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Static Members
        private static readonly Task<DynamicImageItem> s_NullDynamicImageItemResult = Task.FromResult<DynamicImageItem>(null);
        #endregion

        #region Private Members
        private readonly DynamicImageDiskCacheOptions m_DiskCacheOptions;
        private readonly string m_CachePathFormat;
        #endregion

        #region Constructors
        public DynamicImageDiskCache(ILogger<DynamicImageDiskCache> logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageDiskCacheOptions diskCacheOptions)
            : base(logger, cache, cacheOptions)
        {
            if (string.IsNullOrWhiteSpace(diskCacheOptions.PhysicalFolderPath))
                throw new DynamicImageException($"The {diskCacheOptions.PhysicalFolderPath} must be specified.");

            //Sanitize the provided path
            diskCacheOptions.PhysicalFolderPath = diskCacheOptions.PhysicalFolderPath.Trim().TrimEnd('\\');

            m_DiskCacheOptions = diskCacheOptions;
            m_CachePathFormat = diskCacheOptions.PhysicalFolderPath + @"\{0}\{1}.{2}";
        }
        #endregion

        #region IDynamicImageCache Members
        public async Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                //Save to disk - overwrite any existing entry
                string physicalCachedFilePath = GetPhysicalPath(key, dynamicImage.ImageOptions.Format.ToFileExtensionString());

                //We need to ensure the directory structure exists
                string directoryName = Path.GetDirectoryName(physicalCachedFilePath);

                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                
                using (FileStream fs = new FileStream(physicalCachedFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] content = await dynamicImage.GetContentAsync();

                    await fs.WriteAsync(content, 0, content.Length).ConfigureAwait(false);
                }
            }
            catch(Exception exc) when(Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public Task<DynamicImageItem> GetAsync(string key, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string physicalCachedFilePath = GetPhysicalPath(key, fileExtension);

                //Find the cached file
                FileInfo fiCached = new FileInfo(physicalCachedFilePath);

                //No cached image available
                if (!fiCached.Exists)
                    return s_NullDynamicImageItemResult;

                //If the file does not exist or has been modified since the item was generated,
                //evict it from the cache, i.e. delete the cached image from disk
                if (sourceLastModified > fiCached.LastWriteTime)
                {
                    if (File.Exists(physicalCachedFilePath))
                        File.Delete(physicalCachedFilePath);

                    return s_NullDynamicImageItemResult;
                }

                //We need to return the cached image
                DynamicImageItem item = new DynamicImageItem
                {
                    LastModified = fiCached.LastWriteTimeUtc,
                    Length = fiCached.Length
                };

                //Set the content resolver to allow the file to be read from disk if / when needed
                item.SetContentResolver(async () =>
                {
                    //Check the file still exists. Could potentially be removed before this resolver is executed.
                    if (!File.Exists(physicalCachedFilePath))
                        return null;

                    using (FileStream fs = File.OpenRead(physicalCachedFilePath))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await fs.CopyToAsync(ms);
                            return ms.ToArray();
                        }
                    }
                });

                return Task.FromResult(item);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { key, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public Task RemoveAsync(string key, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string physicalCachedFilePath = GetPhysicalPath(key, fileExtension);

                if (File.Exists(physicalCachedFilePath))
                    File.Delete(physicalCachedFilePath);

                return Task.CompletedTask;
            }
            catch (Exception exc) when(Log.WriteError(exc, new { key, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem removing the image from the cache.", exc);
            }
        }
        #endregion

        #region Private Methods
        private string GetPhysicalPath(string cacheKey, string fileExtension)
            => string.Format(m_CachePathFormat, cacheKey.Substring(0, 2), cacheKey, fileExtension);
        #endregion
    }
}