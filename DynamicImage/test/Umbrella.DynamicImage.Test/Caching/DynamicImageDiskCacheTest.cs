using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Xunit;

namespace Umbrella.DynamicImage.Test.Caching
{
    public class DynamicImageDiskCacheTest
    {
        [Fact]
        public async Task AddAsyncTest()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            var cacheOptions = new DynamicImageCacheOptions
            {
                CacheKeyCacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                }
            };

            var diskCacheOptions = new DynamicImageDiskCacheOptions
            {
                PhysicalFolderPath = @"C:\Data\Umbrella\DynamicImageCache"
            };

            DynamicImageDiskCache cache = new DynamicImageDiskCache(null, null, memoryCache, cacheOptions, diskCacheOptions);

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Png,
                    Height = 100,
                    Width = 100,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = @"C:\test.jpg"
                },
                LastModified = DateTime.UtcNow
            };

            item.SetContent(new byte[100]);

            await cache.AddAsync(item);
        }
    }
}