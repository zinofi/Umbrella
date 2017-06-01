using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Umbrella.Utilities.Mime;
using Xunit;
using Umbrella.Utilities.Compilation;
using Umbrella.FileSystem.Disk;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.Utilities.Hosting;
using Umbrella.FileSystem.AzureStorage;

namespace Umbrella.DynamicImage.Test.Caching
{
    public class DynamicImageCacheTest
    {
        //TODO: When moving to GitHub this connection string needs to be dynamically set somehow before executing the tests
#if DEBUG
        private const string c_StorageConnectionString = "UseDevelopmentStorage=true";
#else
        private const string c_StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=umbrellablobtest;AccountKey=eaxPzjIwVy4WQTCUQnUIL6cIYbzFolVp72nfStCQMNXU8lG4I/zaa2ll1wdiZ2q2h4roIA+DCISXnwhD2nRU0A==;EndpointSuffix=core.windows.net";
#endif
        private const string c_TestFileName = "aspnet-mvc-logo.png";

        private static List<IDynamicImageCache> CacheList = new List<IDynamicImageCache>
        {
            CreateDynamicImageMemoryCache(),
            CreateDynamicImageDiskCache(),
            CreateDynamicImageAzureBlobStorageCache()
        };

        public static List<object[]> CacheListMemberData = CacheList.Select(x => new object[] { x }).ToList();

        private static string s_BaseDirectory;

        private static string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(s_BaseDirectory))
                {
                    string buildConfig = DebugUtility.IsDebugMode ? "Debug" : "Release";
                    int indexToEndAt = AppContext.BaseDirectory.IndexOf($@"\bin\{buildConfig}\netcoreapp1.1");
                    s_BaseDirectory = AppContext.BaseDirectory.Remove(indexToEndAt, AppContext.BaseDirectory.Length - indexToEndAt);
                }

                return s_BaseDirectory;
            }
        }

        [Theory]
        [MemberData(nameof(CacheListMemberData))]
        public async Task AddAsync_RemoveAsync_Bytes(IDynamicImageCache cache)
        {
            var physicalPath = $@"{BaseDirectory}\{c_TestFileName}";

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Jpeg,
                    Height = 100,
                    Width = 100,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = "/sometestpath/image.png"
                },
                LastModified = DateTime.UtcNow
            };

            byte[] sourceBytes = File.ReadAllBytes(physicalPath);

            item.Content = sourceBytes;

            await cache.AddAsync(item);

            DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

            Assert.NotNull(cachedItem);
            Assert.Equal(item.ImageOptions, cachedItem.ImageOptions);

            byte[] cachedBytes = await cachedItem.GetContentAsync();

            Assert.Equal(sourceBytes.Length, cachedBytes.Length);

            //Perform cleanup by removing the file from the cache
            await cache.RemoveAsync(item.ImageOptions, "jpg");

            cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

            Assert.Null(cachedItem);
        }

        [Theory]
        [MemberData(nameof(CacheListMemberData))]
        public async Task AddAsync_RemoveAsync_Stream(IDynamicImageCache cache)
        {
            var physicalPath = $@"{BaseDirectory}\{c_TestFileName}";

            var item = new DynamicImageItem
            {
                ImageOptions = new DynamicImageOptions
                {
                    Format = DynamicImageFormat.Jpeg,
                    Height = 100,
                    Width = 100,
                    ResizeMode = DynamicResizeMode.UniformFill,
                    SourcePath = "/sometestpath/image.png"
                },
                LastModified = DateTime.UtcNow
            };

            byte[] sourceBytes = File.ReadAllBytes(physicalPath);

            item.Content = sourceBytes;

            await cache.AddAsync(item);

            DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

            Assert.NotNull(cachedItem);
            Assert.Equal(item.ImageOptions, cachedItem.ImageOptions);

            byte[] cachedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                await cachedItem.WriteContentToStreamAsync(ms);
                cachedBytes = ms.ToArray();
            }

            Assert.Equal(sourceBytes.Length, cachedBytes.Length);

            //Perform cleanup by removing the file from the cache
            await cache.RemoveAsync(item.ImageOptions, "jpg");

            cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

            Assert.Null(cachedItem);
        }

        [Theory]
        [MemberData(nameof(CacheListMemberData))]
        public async Task GetAsync_NotExists(IDynamicImageCache cache)
        {
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

            DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.MinValue, "jpg");

            Assert.Null(cachedItem);
        }

        [Theory]
        [MemberData(nameof(CacheListMemberData))]
        public async Task AddAsync_GetAsync_Expired(IDynamicImageCache cache)
        {
            var path = $@"{BaseDirectory}\{c_TestFileName}";

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

            byte[] sourceBytes = File.ReadAllBytes(path);

            item.Content = sourceBytes;

            await cache.AddAsync(item);

            DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(5), "jpg");

            Assert.Null(cachedItem);
        }

        private static DynamicImageDiskCache CreateDynamicImageDiskCache()
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
                CacheFolderName = "DynamicImageCache"
            };

            var cacheLogger = new Mock<ILogger<DynamicImageDiskCache>>();
            var fileProviderLogger = new Mock<ILogger<UmbrellaDiskFileProvider>>();

            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(UmbrellaDiskFileProvider)))).Returns(fileProviderLogger.Object);

            var mimeTypeUtility = new Mock<IMimeTypeUtility>();
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

            var fileProviderOptions = new UmbrellaDiskFileProviderOptions
            {
                RootPhysicalPath = BaseDirectory
            };

            var fileProvider = new UmbrellaDiskFileProvider(loggerFactory.Object, mimeTypeUtility.Object, fileProviderOptions);

            return new DynamicImageDiskCache(cacheLogger.Object, memoryCache, cacheOptions, fileProvider, diskCacheOptions);
        }

        private static DynamicImageMemoryCache CreateDynamicImageMemoryCache()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            var cacheOptions = new DynamicImageCacheOptions
            {
                CacheKeyCacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                }
            };

            var memoryCacheOptions = new DynamicImageMemoryCacheOptions
            {
                ItemCacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                }
            };

            var cacheLogger = new Mock<ILogger<DynamicImageMemoryCache>>();

            return new DynamicImageMemoryCache(cacheLogger.Object, memoryCache, cacheOptions, memoryCacheOptions);
        }

        private static DynamicImageAzureBlobStorageCache CreateDynamicImageAzureBlobStorageCache()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            var cacheOptions = new DynamicImageCacheOptions
            {
                CacheKeyCacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                }
            };

            var cacheLogger = new Mock<ILogger<DynamicImageAzureBlobStorageCache>>();
            var fileProviderLogger = new Mock<ILogger<DynamicImageAzureBlobStorageCache>>();

            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(UmbrellaAzureBlobFileProvider)))).Returns(fileProviderLogger.Object);

            var mimeTypeUtility = new Mock<IMimeTypeUtility>();
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

            var options = new UmbrellaAzureBlobFileProviderOptions
            {
                StorageConnectionString = c_StorageConnectionString
            };

            var fileProvider = new UmbrellaAzureBlobFileProvider(loggerFactory.Object, mimeTypeUtility.Object, options);

            var blobStorageCacheOptions = new DynamicImageAzureBlobStorageCacheOptions();

            return new DynamicImageAzureBlobStorageCache(cacheLogger.Object, memoryCache, cacheOptions, fileProvider, blobStorageCacheOptions);
        }
    }
}