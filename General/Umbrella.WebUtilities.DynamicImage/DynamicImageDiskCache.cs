using Umbrella.Utilities;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using System.IO;
using System.Runtime.Caching;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageDiskCache : DynamicImageCache, IDynamicImageCache
    {
        #region Constants
        private const string c_CachePath = "~/DynamicImageCache/{0}/{1}.{2}";
        #endregion

        #region Private Members
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
        private readonly ILogger m_Logger;
        #endregion

        #region Constructors
        //TODO: public DynamicImageDiskCache()
        //    : this(null, null)
        //{
        //}

        public DynamicImageDiskCache(IUmbrellaHostingEnvironment hostingEnvironment, ILogger logger)
        {
            m_HostingEnvironment = hostingEnvironment;
            m_Logger = logger;
        }
        #endregion

        #region IDynamicImageCache Members
        public void Add(DynamicImage dynamicImage, Func<CacheItemPolicy> policyFunc = null)
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                //Save to disk - overwrite any existing entry
                string virtualPath = string.Format(c_CachePath, key.Substring(0, 2), key, dynamicImage.ImageOptions.Format.ToFileExtensionString());
                string physicalPath = m_HostingEnvironment.MapPath(virtualPath);

                //Set the virtual path of the cached image
                dynamicImage.CachedVirtualPath = virtualPath;

                //We need to ensure the directory structure exists
                string directoryName = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                using (FileStream fs = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(dynamicImage.Content, 0, dynamicImage.Content.Length);
                }
            }
            catch(Exception exc)
            {
                m_Logger.LogError(exc);
                throw;
            }
        }

        public DynamicImage Get(string key, string originalFilePhysicalPath, string fileExtension)
        {
            try
            {
                string virtualPath = string.Format(c_CachePath, key.Substring(0, 2), key, fileExtension);
                string physicalPath = m_HostingEnvironment.MapPath(virtualPath);

                //Find the cached file
                FileInfo fiCached = new FileInfo(physicalPath);

                //No cached image available
                if (fiCached == null)
                    return null;

                //Find the original file and check to see if it has been changed since the dynamic image
                //was generated
                FileInfo fiOriginal = new FileInfo(originalFilePhysicalPath);

                //If the file does not exist or has been modified since the IDynamicImage was generated,
                //evict it from the cache, i.e. delete the cached image from disk
                if (fiOriginal == null || (fiOriginal != null && fiOriginal.LastWriteTime > fiCached.LastWriteTime || fiOriginal.CreationTime > fiCached.CreationTime))
                {
                    if (File.Exists(physicalPath))
                        File.Delete(physicalPath);

                    return null;
                }

                //We need to return the cached image
                DynamicImage image = new DynamicImage();
                image.LastModified = fiCached.LastWriteTime;

                //Set the virtual path of the cached image
                //We do not read the file into memory though
                image.CachedVirtualPath = virtualPath;

                return image;
            }
            catch(Exception exc)
            {
                m_Logger.LogError(exc, new { key, originalFilePhysicalPath, fileExtension });
                throw;
            }
        }

        public void Remove(string key, string fileExtension)
        {
            try
            {
                string virtualPath = string.Format(c_CachePath, key.Substring(0, 2), key, fileExtension);
                string physicalPath = m_HostingEnvironment.MapPath(virtualPath);

                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }
            catch (Exception exc)
            {
                m_Logger.LogError(exc, new { key, fileExtension });
                throw;
            }
        }
        #endregion
    }
}