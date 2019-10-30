using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.Utilities;

[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Caching.AzureStorage.Test")]

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
	/// <summary>
	/// A Dynamic Image cache implementation that is backed by Azure Blob Storage.
	/// </summary>
	public class DynamicImageAzureBlobStorageCache : DynamicImagePhysicalCache<IUmbrellaAzureBlobStorageFileProvider>, IDynamicImageCache
	{
		#region Protected Properties
		protected DynamicImageAzureBlobStorageCacheOptions BlobStorageCacheOptions { get; }
		#endregion

		#region Constructors
		public DynamicImageAzureBlobStorageCache(ILogger<DynamicImageAzureBlobStorageCache> logger,
			IMemoryCache cache,
			DynamicImageCacheCoreOptions cacheOptions,
			IUmbrellaAzureBlobStorageFileProvider fileProvider,
			DynamicImageAzureBlobStorageCacheOptions blobStorageCacheOptions)
			: base(logger, cache, cacheOptions, fileProvider)
		{
			Guard.ArgumentNotNullOrWhiteSpace(blobStorageCacheOptions.ContainerName, nameof(blobStorageCacheOptions.ContainerName));
			
			BlobStorageCacheOptions = blobStorageCacheOptions;
		}
		#endregion

		#region Overridden Methods
		protected override string GetSubPath(string cacheKey, string fileExtension)
			=> $@"\{BlobStorageCacheOptions.ContainerName}{base.GetSubPath(cacheKey, fileExtension)}";
		#endregion
	}
}