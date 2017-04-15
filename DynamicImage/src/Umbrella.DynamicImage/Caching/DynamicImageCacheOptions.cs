using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageCacheOptions
    {
        public MemoryCacheEntryOptions CacheKeyCacheOptions { get; set; }
    }
}