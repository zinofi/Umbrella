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
using System.Runtime.Caching;
using Umbrella.Utilities.Hosting;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageMemoryCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Static Members
        private static readonly MemoryCache<string, DynamicImage> s_Cache = new MemoryCache<string, DynamicImage>();
        #endregion

        #region Private Members
        private readonly Func<CacheItemPolicy> m_DefaultPolicyFunc;
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
        #endregion

        #region Constructors
        public DynamicImageMemoryCache(Func<CacheItemPolicy> defaultPolicyFunc = null, IUmbrellaHostingEnvironment hostingEnvironment = null)
        {
            m_DefaultPolicyFunc = defaultPolicyFunc ?? (() => new CacheItemPolicy());
            m_HostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region IDynamicImageCache Members
        public void Add(DynamicImage dynamicImage, Func<CacheItemPolicy> policyFunc = null)
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);
                s_Cache.AddOrGet(key, () => dynamicImage, policyFunc ?? m_DefaultPolicyFunc);
            }
            catch(Exception exc)
            {
                //TODO: Log.Error("Add() failed", exc);
                throw;
            }
        }

        public DynamicImage Get(string key, string originalFilePhysicalPath, string fileExtension)
        {
            try
            {
                DynamicImage item = s_Cache.Get(key);

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
                        s_Cache.Remove(key);

                        return null;
                    }
                }

                return item;
            }
            catch(Exception exc)
            {
                //TODO: Log.Error(string.Format("Get({0}, {1}, {2}) failed", key, originalFilePhysicalPath, fileExtension), exc);
                throw;
            }
        }

        public void Remove(string key, string fileExtension)
        {
            try
            {
                s_Cache.Remove(key);
            }
            catch(Exception exc)
            {
                //TODO: Log.Error(string.Format("Remove({0}, {1}) failed", key, fileExtension), exc);
                throw;
            }
        }
        #endregion
    }
}