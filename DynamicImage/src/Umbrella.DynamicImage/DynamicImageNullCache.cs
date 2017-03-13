using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage
{
    public class DynamicImageNullCache : IDynamicImageCache
    {
        public void Add(DynamicImageItem dynamicImage, Func<MemoryCacheEntryOptions> options)
        {

        }

        public DynamicImageItem Get(string key, string originalFilePhysicalPath, string fileExtension)
        {
            return null;
        }

        public void Remove(string key, string fileExtension)
        {

        }

        public string GenerateCacheKey(DynamicImageOptions options)
        {
            return string.Empty;
        }
    }
}
