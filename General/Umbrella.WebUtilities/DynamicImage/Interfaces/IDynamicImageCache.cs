using Umbrella.WebUtilities.DynamicImage.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Umbrella.WebUtilities.DynamicImage.Interfaces
{
    public interface IDynamicImageCache
    {
        void Add(DynamicImage dynamicImage, Func<CacheItemPolicy> policyFunc = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalFilePhysicalPath"></param>
        /// <param name="fileExtension"></param>
        /// <param name="readFileIntoMemory">Only applicable when using the disk based cache. The memory cache already has the file in memory.</param>
        /// <returns></returns>
        DynamicImage Get(string key, string originalFilePhysicalPath, string fileExtension);
        void Remove(string key, string fileExtension);
        string GenerateCacheKey(DynamicImageOptions options);
    }
}
