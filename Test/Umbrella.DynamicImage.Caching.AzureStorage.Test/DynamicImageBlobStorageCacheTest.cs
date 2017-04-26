using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Mime;
using Xunit;

namespace Umbrella.DynamicImage.Caching.AzureStorage.Test
{
    public class DynamicImageBlobStorageCacheTest
    {
        //TODO: When moving to GitHub this connection string needs to be dynamically set somehow before executing the tests
        private const string c_StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=umbrellablobtest;AccountKey=eaxPzjIwVy4WQTCUQnUIL6cIYbzFolVp72nfStCQMNXU8lG4I/zaa2ll1wdiZ2q2h4roIA+DCISXnwhD2nRU0A==;EndpointSuffix=core.windows.net";

        private string m_BaseDirectory;

        private string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(m_BaseDirectory))
                {
                    int indexToEndAt = AppContext.BaseDirectory.IndexOf(@"\bin\Debug\netcoreapp1.1");
                    m_BaseDirectory = AppContext.BaseDirectory.Remove(indexToEndAt, AppContext.BaseDirectory.Length - indexToEndAt);
                }

                return m_BaseDirectory;
            }
        }

        [Fact]
        public async Task AddAsync_RemoveAsync()
        {
            var cache = CreateDynamicImageBlobStorageCache();
            
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

            //Verify the file exists in blob storage
            string cacheKey = cache.GenerateCacheKey(item.ImageOptions);

            CloudBlockBlob cachedBlob = cache.BlobContainer.GetBlockBlobReference($"{cacheKey}.jpg");
            bool exists = await cachedBlob.ExistsAsync();

            Assert.True(exists);

            //Perform cleanup by removing the file from the disk cache
            await cache.RemoveAsync(cacheKey, "jpg");

            //Verify the file has been removed from blob storage
            exists = await cachedBlob.ExistsAsync();

            Assert.False(exists);
        }

        [Fact]
        public async Task GetAsync_NotExists()
        {
            var cache = CreateDynamicImageBlobStorageCache();

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
            var cache = CreateDynamicImageBlobStorageCache();

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

        private DynamicImageBlobStorageCache CreateDynamicImageBlobStorageCache()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            var cacheOptions = new DynamicImageCacheOptions
            {
                CacheKeyCacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                }
            };

            var blobCacheOptions = new DynamicImageBlobStorageCacheOptions
            {
                ContainerName = "dynamicimagecache",
                StorageConnectionString = c_StorageConnectionString
            };

            var logger = new Mock<ILogger<DynamicImageBlobStorageCache>>();
            var mimeTypeUtility = new Mock<IMimeTypeUtility>();
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

            return new DynamicImageBlobStorageCache(logger.Object, mimeTypeUtility.Object, memoryCache, cacheOptions, blobCacheOptions);
        }
    }
}