using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageMemoryCacheOptions
    {
        /// <summary>
        /// The cache options that will be applied to image items stored in the MemoryCache.
        /// </summary>
        public MemoryCacheEntryOptions ItemCacheOptions { get; set; }
    }
}