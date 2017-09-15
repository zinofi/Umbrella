using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Extensions;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Umbrella.Utilities;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Umbrella.Utilities.Mime;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;

[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Caching.AzureStorage.Test")]

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
    /// <summary>
    /// A Dynamic Image cache implementation that is backed by Azure Blob Storage.
    /// </summary>
    public class DynamicImageAzureBlobStorageCache : DynamicImagePhysicalCache, IDynamicImageCache
    {
        #region Protected Properties
        protected DynamicImageAzureBlobStorageCacheOptions BlobStorageCacheOptions { get; }
        #endregion

        #region Constructors
        public DynamicImageAzureBlobStorageCache(ILogger<DynamicImageAzureBlobStorageCache> logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
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