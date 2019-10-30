using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;
using Xunit;

namespace Umbrella.DynamicImage.Test.Caching
{
	public class DynamicImageCacheTest
	{
#if AZUREDEVOPS
        private static readonly string StorageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
#else
		private const string StorageConnectionString = "UseDevelopmentStorage=true";
#endif
		private const string TestFileName = "aspnet-mvc-logo.png";

		private static readonly List<IDynamicImageCache> CacheList = new List<IDynamicImageCache>
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
					string baseDirectory = AppContext.BaseDirectory.ToLowerInvariant();
					int indexToEndAt = baseDirectory.IndexOf($@"\bin\{DebugUtility.BuildConfiguration}\netcoreapp3.0");
					s_BaseDirectory = baseDirectory.Remove(indexToEndAt, baseDirectory.Length - indexToEndAt);
				}

				return s_BaseDirectory;
			}
		}

		[Theory]
		[MemberData(nameof(CacheListMemberData))]
		public async Task AddAsync_RemoveAsync_Bytes(IDynamicImageCache cache)
		{
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions("/sometestpath/image.png", 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
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
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions("/sometestpath/image.png", 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			byte[] sourceBytes = File.ReadAllBytes(physicalPath);

			item.Content = sourceBytes;

			await cache.AddAsync(item);

			DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

			Assert.NotNull(cachedItem);
			Assert.Equal(item.ImageOptions, cachedItem.ImageOptions);

			byte[] cachedBytes = null;

			using (var ms = new MemoryStream())
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
			string path = $@"{BaseDirectory}\doesnotexist.png";

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions(path, 200, 200, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			DynamicImageItem cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.MinValue, "jpg");

			Assert.Null(cachedItem);
		}

		[Theory]
		[MemberData(nameof(CacheListMemberData))]
		public async Task AddAsync_GetAsync_Expired(IDynamicImageCache cache)
		{
			string path = $@"{BaseDirectory}\{TestFileName}";

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions(path, 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
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

			var cacheOptions = new DynamicImageCacheCoreOptions
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
			loggerFactory.Setup(x => x.CreateLogger(typeof(UmbrellaDiskFileProvider).FullName)).Returns(fileProviderLogger.Object);

			var mimeTypeUtility = new Mock<IMimeTypeUtility>();
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.TrimToLowerInvariant().EndsWith("png")))).Returns("image/png");
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.TrimToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

			var genericTypeConverter = new Mock<IGenericTypeConverter>();

			var fileProviderOptions = new UmbrellaDiskFileProviderOptions
			{
				RootPhysicalPath = BaseDirectory
			};

			var fileProvider = new UmbrellaDiskFileProvider(loggerFactory.Object, mimeTypeUtility.Object, genericTypeConverter.Object, fileProviderOptions);

			return new DynamicImageDiskCache(cacheLogger.Object, memoryCache, cacheOptions, fileProvider, diskCacheOptions);
		}

		private static DynamicImageMemoryCache CreateDynamicImageMemoryCache()
		{
			var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

			var cacheOptions = new DynamicImageCacheCoreOptions
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

			var cacheOptions = new DynamicImageCacheCoreOptions
			{
				CacheKeyCacheOptions = new MemoryCacheEntryOptions
				{
					SlidingExpiration = TimeSpan.FromHours(1)
				}
			};

			var cacheLogger = new Mock<ILogger<DynamicImageAzureBlobStorageCache>>();
			var fileProviderLogger = new Mock<ILogger<DynamicImageAzureBlobStorageCache>>();

			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(typeof(UmbrellaAzureBlobStorageFileProvider).FullName)).Returns(fileProviderLogger.Object);

			var mimeTypeUtility = new Mock<IMimeTypeUtility>();
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

			var genericTypeConverter = new Mock<IGenericTypeConverter>();

			var options = new UmbrellaAzureBlobStorageFileProviderOptions
			{
				StorageConnectionString = StorageConnectionString
			};

			var fileProvider = new UmbrellaAzureBlobStorageFileProvider(loggerFactory.Object, mimeTypeUtility.Object, genericTypeConverter.Object, options);

			var blobStorageCacheOptions = new DynamicImageAzureBlobStorageCacheOptions();

			return new DynamicImageAzureBlobStorageCache(cacheLogger.Object, memoryCache, cacheOptions, fileProvider, blobStorageCacheOptions);
		}
	}
}