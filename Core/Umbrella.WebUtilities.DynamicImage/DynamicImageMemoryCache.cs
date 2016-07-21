using Umbrella.Utilities;
using Umbrella.Utilities.Caching;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageMemoryCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Members
        private readonly IMemoryCache m_Cache;
        private readonly MemoryCacheEntryOptions m_DefaultMemoryCacheEntryOptions;
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
        #endregion

        #region Constructors
        public DynamicImageMemoryCache(ILogger<DynamicImageMemoryCache> logger,
            IMemoryCache cache,
            MemoryCacheEntryOptions defaultMemoryCacheEntryOptions = null,
            IUmbrellaHostingEnvironment hostingEnvironment = null)
            : base(logger)
        {
            m_Cache = cache;
            m_DefaultMemoryCacheEntryOptions = defaultMemoryCacheEntryOptions ?? new MemoryCacheEntryOptions();
            m_HostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region IDynamicImageCache Members
        public void Add(DynamicImage dynamicImage, Func<MemoryCacheEntryOptions> options = null)
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                m_Cache.GetOrCreate(key, entry =>
                {
                    entry.SetOptions(options?.Invoke() ?? m_DefaultMemoryCacheEntryOptions);

                    return dynamicImage;
                });
            }
            catch(Exception exc) when (m_Logger.WriteError(exc))
            {
                throw;
            }
        }

        public DynamicImage Get(string key, string originalFilePhysicalPath, string fileExtension)
        {
            try
            {
                DynamicImage item = m_Cache.Get<DynamicImage>(key);

                if (item != null)
                {
                    //Find the original file and check to see if it has been changed since the dynamic image
                    //was generated
                    string physicalPath = m_HostingEnvironment.MapPath(item.ImageOptions.OriginalVirtualPath);

                    FileInfo fi = new FileInfo(physicalPath);

                    //If the file does not exist or has been modified since the IDynamicImage was generated,
                    //evict it from the cache
                    if (fi == null || (fi != null && fi.LastWriteTime > item.LastModified || fi.CreationTime > item.LastModified))
                    {
                        m_Cache.Remove(key);

                        return null;
                    }
                }

                return item;
            }
            catch(Exception exc) when (m_Logger.WriteError(exc, new { key, originalFilePhysicalPath, fileExtension }))
            {
                throw;
            }
        }

        public void Remove(string key, string fileExtension)
        {
            try
            {
                m_Cache.Remove(key);
            }
            catch(Exception exc) when (m_Logger.WriteError(exc, new { key, fileExtension }))
            {
                throw;
            }
        }
        #endregion
    }
}