using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.DynamicImage.Caching
{
	public class DynamicImageDiskCache : DynamicImagePhysicalCache<IUmbrellaDiskFileProvider>, IDynamicImageCache
	{
		#region Protected Properties
		protected DynamicImageDiskCacheOptions DiskCacheOptions { get; }
		#endregion

		#region Constructors
		public DynamicImageDiskCache(ILogger<DynamicImageDiskCache> logger,
			IHybridCache cache,
			ICacheKeyUtility cacheKeyUtility,
			DynamicImageCacheCoreOptions cacheOptions,
			IUmbrellaDiskFileProvider fileProvider,
			DynamicImageDiskCacheOptions diskCacheOptions)
			: base(logger, cache, cacheKeyUtility, cacheOptions, fileProvider)
		{
			Guard.ArgumentNotNullOrWhiteSpace(diskCacheOptions.CacheFolderName, nameof(diskCacheOptions.CacheFolderName));

			DiskCacheOptions = diskCacheOptions;
		}
		#endregion

		#region Overridden Methods
		protected override string GetSubPath(string cacheKey, string fileExtension)
			=> $@"\{DiskCacheOptions.CacheFolderName}\{cacheKey.Substring(0, 2)}{base.GetSubPath(cacheKey, fileExtension)}";
		#endregion
	}
}