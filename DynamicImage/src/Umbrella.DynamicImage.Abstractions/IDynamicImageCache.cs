using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageCache
    {
        void Add(DynamicImageItem dynamicImage, Func<MemoryCacheEntryOptions> options = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalFilePhysicalPath"></param>
        /// <param name="fileExtension"></param>
        /// <param name="readFileIntoMemory">Only applicable when using the disk based cache. The memory cache already has the file in memory.</param>
        /// <returns></returns>
        DynamicImageItem Get(string key, string originalFilePhysicalPath, string fileExtension);
        void Remove(string key, string fileExtension);
        string GenerateCacheKey(DynamicImageOptions options);
    }
}
