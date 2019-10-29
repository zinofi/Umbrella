using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageDiskCache : DynamicImagePhysicalCache, IDynamicImageCache
    {
        #region Protected Properties
        protected DynamicImageDiskCacheOptions DiskCacheOptions { get; }
        #endregion

        #region Constructors
        public DynamicImageDiskCache(ILogger<DynamicImageDiskCache> logger,
            IMemoryCache cache,
            DynamicImageCacheCoreOptions cacheOptions,
            IUmbrellaDiskFileProvider fileProvider,
            DynamicImageDiskCacheOptions diskCacheOptions)
            : base(logger, cache, cacheOptions, fileProvider)
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