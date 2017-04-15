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

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageDiskCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Members
        private readonly IUmbrellaHostingEnvironment m_UmbrellaHostingEnvironment;
        private readonly DynamicImageDiskCacheOptions m_DiskCacheOptions;
        private readonly string m_CachePathFormat;
        #endregion

        #region Constructors
        public DynamicImageDiskCache(IUmbrellaHostingEnvironment umbrellaHostingEnvironment,
            ILogger<DynamicImageDiskCache> logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageDiskCacheOptions diskCacheOptions)
            : base(logger, cache, cacheOptions)
        {
            if (string.IsNullOrWhiteSpace(diskCacheOptions.PhysicalFolderPath))
                throw new DynamicImageException($"The {diskCacheOptions.PhysicalFolderPath} must be specified.");

            //Sanitize the provided path
            diskCacheOptions.PhysicalFolderPath = diskCacheOptions.PhysicalFolderPath.Trim().TrimEnd('\\');

            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
            m_DiskCacheOptions = diskCacheOptions;
            m_CachePathFormat = diskCacheOptions.PhysicalFolderPath.Trim().TrimEnd('\\') + @"\{0}\{1}.{2}";
        }
        #endregion

        #region IDynamicImageCache Members
        public async Task AddAsync(DynamicImageItem dynamicImage)
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
            catch(Exception exc) when(Log.WriteError(exc, new { dynamicImage.ImageOptions }))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public Task<DynamicImageItem> GetAsync(string key, DateTime sourceLastModified, string fileExtension)
        {
            try
            {
                string physicalCachedFilePath = GetPhysicalPath(key, fileExtension);

                //Find the cached file
                FileInfo fiCached = new FileInfo(physicalCachedFilePath);
                
                //No cached image available
                if (fiCached == null)
                    return null;

                //If the file does not exist or has been modified since the item was generated,
                //evict it from the cache, i.e. delete the cached image from disk
                if (sourceLastModified > fiCached.LastWriteTime)
                {
                    if (File.Exists(physicalCachedFilePath))
                        File.Delete(physicalCachedFilePath);

                    return null;
                }

                //We need to return the cached image
                DynamicImageItem item = new DynamicImageItem
                {
                    LastModified = fiCached.LastWriteTime
                };

                //Set the content resolver to allow the file to be read from disk if / when needed
                item.SetContentResolver(() => Task.FromResult(File.ReadAllBytes(physicalCachedFilePath)));

                return Task.FromResult(item);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { key, sourceLastModified, fileExtension }))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public Task RemoveAsync(string key, string fileExtension)
        {
            try
            {
                string physicalCachedFilePath = GetPhysicalPath(key, fileExtension);

                if (File.Exists(physicalCachedFilePath))
                    File.Delete(physicalCachedFilePath);

                return Task.CompletedTask;
            }
            catch (Exception exc) when(Log.WriteError(exc, new { key, fileExtension }))
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