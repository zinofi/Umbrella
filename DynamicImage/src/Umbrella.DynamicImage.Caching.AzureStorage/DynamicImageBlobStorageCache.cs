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

[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Caching.AzureStorage.Test")]

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
    public class DynamicImageBlobStorageCache : DynamicImageCache, IDynamicImageCache
    {
        #region Private Members
        private readonly DynamicImageBlobStorageCacheOptions m_BlobStorageCacheOptions;
        private readonly CloudStorageAccount m_StorageAccount;
        private readonly CloudBlobClient m_BlobClient;
        #endregion

        #region Internal Properties
        internal CloudBlobContainer BlobContainer { get; }
        #endregion

        #region Constructors
        public DynamicImageBlobStorageCache(ILogger<DynamicImageBlobStorageCache> logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageBlobStorageCacheOptions blobStorageCacheOptions)
            : base(logger, cache, cacheOptions)
        {
            m_BlobStorageCacheOptions = blobStorageCacheOptions;

            Guard.ArgumentNotNullOrWhiteSpace(blobStorageCacheOptions.ContainerName, nameof(blobStorageCacheOptions.ContainerName));
            Guard.ArgumentNotNullOrWhiteSpace(blobStorageCacheOptions.StorageConnectionString, nameof(blobStorageCacheOptions.StorageConnectionString));

            m_StorageAccount = CloudStorageAccount.Parse(blobStorageCacheOptions.StorageConnectionString);
            m_BlobClient = m_StorageAccount.CreateCloudBlobClient();
            BlobContainer = m_BlobClient.GetContainerReference(blobStorageCacheOptions.ContainerName);
        }
        #endregion

        #region IDynamicImageCache Members
        public async Task AddAsync(DynamicImageItem dynamicImage)
        {
            try
            {
                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                await BlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

                CloudBlockBlob blob = GetBlob(key, dynamicImage.ImageOptions.Format.ToFileExtensionString());

                byte[] content = await dynamicImage.GetContentAsync().ConfigureAwait(false);

                await blob.UploadFromByteArrayAsync(content, 0, content.Length).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public async Task<DynamicImageItem> GetAsync(string key, DateTime sourceLastModified, string fileExtension)
        {
            try
            {
                await BlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

                CloudBlockBlob blob = GetBlob(key, fileExtension);
                
                if (!await blob.ExistsAsync())
                    return null;

                DateTime cacheLastModified = blob.Properties.LastModified.Value.UtcDateTime;
                
                if (sourceLastModified > cacheLastModified)
                {
                    //The cache is invalid at this point. Just return null rather than deleting the blob
                    //as the AddAsync will just overwrite the existing item's content.
                    return null;
                }

                DynamicImageItem item = new DynamicImageItem
                {
                    LastModified = cacheLastModified,
                    Length = blob.Properties.Length
                };

                //Set the content resolver to allow the file to be downloaded from blob storage if needed.
                item.SetContentResolver(async () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        await blob.DownloadToStreamAsync(ms).ConfigureAwait(false);
                        return ms.ToArray();
                    }
                });

                return item;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { key, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public Task RemoveAsync(string key, string fileExtension)
        {
            try
            {
                CloudBlockBlob blob = GetBlob(key, fileExtension);

                return blob.DeleteIfExistsAsync();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { key, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem removing the image from the cache.", exc);
            }
        }
        #endregion

        #region Private Methods
        private CloudBlockBlob GetBlob(string cacheKey, string fileExtension)
            => BlobContainer.GetBlockBlobReference($"{cacheKey}.{fileExtension}");
        #endregion
    }
}