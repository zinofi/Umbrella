using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Umbrella.Utilities.Mime;
using Xunit;
using Umbrella.Utilities.Compilation;

namespace Umbrella.DynamicImage.Test.Caching
{
    public class DynamicImageDiskCacheTest
    {
        private string m_BaseDirectory;

        private string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(m_BaseDirectory))
                {
                    string buildConfig = DebugUtility.IsDebugMode ? "Debug" : "Release";
                    int indexToEndAt = AppContext.BaseDirectory.IndexOf($@"\bin\{buildConfig}\netcoreapp1.1");
                    m_BaseDirectory = AppContext.BaseDirectory.Remove(indexToEndAt, AppContext.BaseDirectory.Length - indexToEndAt);
                }

                return m_BaseDirectory;
            }
        }

        [Fact]
        public async Task AddAsync_RemoveAsync()
        {
            var cache = CreateDynamicImageDiskCache();

            var path = $@"{BaseDirectory}\aspnet-mvc-logo.png";

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Jpeg,
                    Height = 100,
                    Width = 100,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = path
                },
                LastModified = DateTime.UtcNow
            };

            item.SetContent(File.ReadAllBytes(path));

            await cache.AddAsync(item);

            //Verify the file exists in the cache on disk as a jpg
            var cacheKey = cache.GenerateCacheKey(item.ImageOptions);
            string cachedPath = $@"{BaseDirectory}\DynamicImageCache\{cacheKey.Substring(0, 2)}\{cacheKey}.jpg";

            Assert.True(File.Exists(cachedPath));

            //Perform cleanup by removing the file from the disk cache
            await cache.RemoveAsync(cacheKey, "jpg");

            Assert.False(File.Exists(cachedPath));
        }

        [Fact]
        public async Task GetAsync_NotExists()
        {
            var cache = CreateDynamicImageDiskCache();

            var path = $@"{BaseDirectory}\doesnotexist.png";

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Jpeg,
                    Height = 200,
                    Width = 200,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = path
                },
                LastModified = DateTime.UtcNow
            };

            string cacheKey = cache.GenerateCacheKey(item.ImageOptions);

            DynamicImageItem cachedItem = await cache.GetAsync(cacheKey, DateTime.MinValue, "jpg");

            Assert.Null(cachedItem);
        }

        [Fact]
        public async Task AddAsync_GetAsync()
        {
            var cache = CreateDynamicImageDiskCache();

            var path = $@"{BaseDirectory}\aspnet-mvc-logo.png";

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Jpeg,
                    Height = 200,
                    Width = 200,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = path
                },
                LastModified = DateTime.UtcNow
            };

            item.SetContent(File.ReadAllBytes(path));

            await cache.AddAsync(item);

            string cacheKey = cache.GenerateCacheKey(item.ImageOptions);

            DynamicImageItem cachedItem = await cache.GetAsync(cacheKey, DateTime.MinValue, "jpg");

            Assert.NotNull(cachedItem);

            //Load the content
            byte[] content = await cachedItem.GetContentAsync();

            Assert.NotEmpty(content);

            //Cleanup
            await cache.RemoveAsync(cacheKey, "jpg");
        }

        private DynamicImageDiskCache CreateDynamicImageDiskCache()
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
                PhysicalFolderPath = $@"{BaseDirectory}\DynamicImageCache"
            };

            var logger = new Mock<ILogger<DynamicImageDiskCache>>();

            var mimeTypeUtility = new Mock<IMimeTypeUtility>();
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

            return new DynamicImageDiskCache(logger.Object, mimeTypeUtility.Object, memoryCache, cacheOptions, diskCacheOptions);
        }
    }
}