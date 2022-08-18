// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Abstractions.Caching;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.DynamicImage.Caching.Disk;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.FileSystem.Disk;
using Umbrella.Internal.Mocks;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Helpers;
using Xunit;

namespace Umbrella.DynamicImage.Test.Caching
{
	public class DynamicImageCacheTest
	{
#if AZUREDEVOPS
        private static readonly string StorageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString")!;
#else
		private const string StorageConnectionString = "UseDevelopmentStorage=true";
#endif
		private const string TestFileName = "aspnet-mvc-logo.png";

		private static readonly List<IDynamicImageCache> CacheList = new()
		{
			CreateDynamicImageMemoryCache(),
			CreateDynamicImageDiskCache(),
			CreateDynamicImageAzureBlobStorageCache()
		};

		public static List<object[]> CacheListMemberData = CacheList.Select(x => new object[] { x }).ToList();

		private static string? s_BaseDirectory;

		private static string BaseDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(s_BaseDirectory))
				{
					string baseDirectory = AppContext.BaseDirectory.ToLowerInvariant();
					int indexToEndAt = baseDirectory.IndexOf(PathHelper.PlatformNormalize($@"\bin\{DebugUtility.BuildConfiguration}\net6.0"));
					s_BaseDirectory = baseDirectory.Remove(indexToEndAt, baseDirectory.Length - indexToEndAt);
				}

				return s_BaseDirectory;
			}
		}

		[Theory]
		[MemberData(nameof(CacheListMemberData))]
		public async Task AddAsync_RemoveAsync_Bytes(IDynamicImageCache cache)
		{
			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions("/sometestpath/image.png", 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			byte[] sourceBytes = File.ReadAllBytes(physicalPath);

			item.Content = sourceBytes;

			await cache.AddAsync(item);

			DynamicImageItem? cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

			Assert.NotNull(cachedItem);
			Assert.Equal(item.ImageOptions, cachedItem!.ImageOptions);

			byte[]? cachedBytes = await cachedItem.GetContentAsync();

			Assert.NotNull(cachedBytes);
			Assert.Equal(sourceBytes.Length, cachedBytes!.Length);

			//Perform cleanup by removing the file from the cache
			await cache.RemoveAsync(item.ImageOptions, "jpg");

			cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

			Assert.Null(cachedItem);
		}

		[Theory]
		[MemberData(nameof(CacheListMemberData))]
		public async Task AddAsync_RemoveAsync_Stream(IDynamicImageCache cache)
		{
			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions("/sometestpath/image.png", 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			byte[] sourceBytes = File.ReadAllBytes(physicalPath);

			item.Content = sourceBytes;

			await cache.AddAsync(item);

			DynamicImageItem? cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(-5), "jpg");

			Assert.NotNull(cachedItem);
			Assert.Equal(item.ImageOptions, cachedItem!.ImageOptions);

			byte[]? cachedBytes = null;

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
			string path = PathHelper.PlatformNormalize($@"{BaseDirectory}\doesnotexist.png");

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions(path, 200, 200, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			DynamicImageItem? cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.MinValue, "jpg");

			Assert.Null(cachedItem);
		}

		[Theory]
		[MemberData(nameof(CacheListMemberData))]
		public async Task AddAsync_GetAsync_Expired(IDynamicImageCache cache)
		{
			string path = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			var item = new DynamicImageItem
			{
				ImageOptions = new DynamicImageOptions(path, 100, 100, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg),
				LastModified = DateTime.UtcNow
			};

			byte[] sourceBytes = File.ReadAllBytes(path);

			item.Content = sourceBytes;

			await cache.AddAsync(item);

			DynamicImageItem? cachedItem = await cache.GetAsync(item.ImageOptions, DateTime.UtcNow.AddMinutes(5), "jpg");

			Assert.Null(cachedItem);
		}

		private static DynamicImageDiskCache CreateDynamicImageDiskCache()
		{
			var options = new UmbrellaDiskFileProviderOptions
			{
				RootPhysicalPath = BaseDirectory
			};

			var provider = new UmbrellaDiskFileProvider(
				CoreUtilitiesMocks.CreateLoggerFactory<UmbrellaDiskFileProvider>(),
				CoreUtilitiesMocks.CreateMimeTypeUtility(("png", "image/png"), ("jpg,", "image/jpg")),
				CoreUtilitiesMocks.CreateGenericTypeConverter());

			provider.InitializeOptions(options);

			return new DynamicImageDiskCache(
				CoreUtilitiesMocks.CreateLogger<DynamicImageDiskCache>(),
				CoreUtilitiesMocks.CreateHybridCache(),
				CoreUtilitiesMocks.CreateCacheKeyUtility(),
				new DynamicImageCacheCoreOptions(),
				provider,
				new DynamicImageDiskCacheOptions());
		}

		private static DynamicImageMemoryCache CreateDynamicImageMemoryCache() => new(
				CoreUtilitiesMocks.CreateLogger<DynamicImageMemoryCache>(),
				CoreUtilitiesMocks.CreateHybridCache(),
				CoreUtilitiesMocks.CreateCacheKeyUtility(),
				new DynamicImageCacheCoreOptions(),
				new DynamicImageMemoryCacheOptions());

		private static DynamicImageAzureBlobStorageCache CreateDynamicImageAzureBlobStorageCache()
		{
			var options = new UmbrellaAzureBlobStorageFileProviderOptions
			{
				StorageConnectionString = StorageConnectionString
			};

			var provider = new UmbrellaAzureBlobStorageFileProvider(
				CoreUtilitiesMocks.CreateLoggerFactory<UmbrellaAzureBlobStorageFileProvider>(),
				CoreUtilitiesMocks.CreateMimeTypeUtility(("png", "image/png"), ("jpg,", "image/jpg")),
				CoreUtilitiesMocks.CreateGenericTypeConverter());

			provider.InitializeOptions(options);

			var blobStorageCacheOptions = new DynamicImageAzureBlobStorageCacheOptions();

			return new DynamicImageAzureBlobStorageCache(
				CoreUtilitiesMocks.CreateLogger<DynamicImageAzureBlobStorageCache>(),
				CoreUtilitiesMocks.CreateHybridCache(),
				CoreUtilitiesMocks.CreateCacheKeyUtility(),
				new DynamicImageCacheCoreOptions(),
				provider,
				blobStorageCacheOptions);
		}
	}
}