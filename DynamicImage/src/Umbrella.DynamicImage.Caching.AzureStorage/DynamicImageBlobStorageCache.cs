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

[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Caching.AzureStorage.Test")]

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
    public class DynamicImageBlobStorageCache : DynamicImageCache, IDynamicImageCache
    {
        #region Protected Properties
        protected DynamicImageBlobStorageCacheOptions BlobStorageCacheOptions { get; }
        protected CloudStorageAccount StorageAccount { get; }
        protected CloudBlobClient BlobClient { get; }
        #endregion

        #region Internal Properties
        internal CloudBlobContainer BlobContainer { get; }
        #endregion

        #region Constructors
        public DynamicImageBlobStorageCache(ILogger<DynamicImageBlobStorageCache> logger,
            IMimeTypeUtility mimeTypeUtility,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            DynamicImageBlobStorageCacheOptions blobStorageCacheOptions)
            : base(logger, mimeTypeUtility, cache, cacheOptions)
        {
            BlobStorageCacheOptions = blobStorageCacheOptions;

            Guard.ArgumentNotNullOrWhiteSpace(blobStorageCacheOptions.ContainerName, nameof(blobStorageCacheOptions.ContainerName));
            Guard.ArgumentNotNullOrWhiteSpace(blobStorageCacheOptions.StorageConnectionString, nameof(blobStorageCacheOptions.StorageConnectionString));

            StorageAccount = CloudStorageAccount.Parse(blobStorageCacheOptions.StorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
            BlobContainer = BlobClient.GetContainerReference(blobStorageCacheOptions.ContainerName);
        }
        #endregion

        #region IDynamicImageCache Members
        public async Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //TODO: Rework all the code in this file to use the appropriate IUmbrellaFileProvider for blob storage
                cancellationToken.ThrowIfCancellationRequested();

                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                await BlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

                string fileExtension = dynamicImage.ImageOptions.Format.ToFileExtensionString();

                CloudBlockBlob blob = GetBlob(key, fileExtension);

                byte[] content = await dynamicImage.GetContentAsync().ConfigureAwait(false);

                blob.Properties.ContentType = MimeTypeUtility.GetMimeType(fileExtension);
                await blob.UploadFromByteArrayAsync(content, 0, content.Length).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public async Task<DynamicImageItem> GetAsync(string key, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await BlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

                CloudBlockBlob blob = GetBlob(key, fileExtension);
                
                //The call to ExistsAsync should populate the properties of the blob
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
                item.SetContentResolver(async token =>
                {
                    token.ThrowIfCancellationRequested();

                    //TODO: Need to pass the token to the download method
                    byte[] bytes = new byte[blob.Properties.Length];
                    await blob.DownloadToByteArrayAsync(bytes, 0).ConfigureAwait(false);

                    return bytes;
                });

                return item;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { key, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public async Task RemoveAsync(string key, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                CloudBlockBlob blob = GetBlob(key, fileExtension);

                await blob.DeleteIfExistsAsync().ConfigureAwait(false);
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