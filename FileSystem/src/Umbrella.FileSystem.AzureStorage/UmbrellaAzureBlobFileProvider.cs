using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.FileSystem.AzureStorage
{
    public class UmbrellaAzureBlobFileProvider : IUmbrellaFileProvider
    {
        private static readonly char[] s_DirectorySeparatorArray = new[] { '/' };

        protected ILogger Log { get; }
        protected ILoggerFactory LoggerFactory { get; }
        protected UmbrellaAzureBlobFileProviderOptions Options { get; }
        protected CloudStorageAccount StorageAccount { get; }
        protected CloudBlobClient BlobClient { get; }

        public UmbrellaAzureBlobFileProvider(ILoggerFactory loggerFactory, UmbrellaAzureBlobFileProviderOptions options)
        {
            LoggerFactory = loggerFactory;
            Log = loggerFactory.CreateLogger<UmbrellaAzureBlobFileProvider>();
            Options = options;

            Guard.ArgumentNotNullOrWhiteSpace(options.StorageConnectionString, nameof(options.StorageConnectionString));

            StorageAccount = CloudStorageAccount.Parse(options.StorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public async Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

                return await GetFileAsync(subpath, true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }))
            {
                throw;
            }
        }

        public async Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

                return await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }))
            {
                throw;
            }
        }

        private async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] parts = subpath.Trim().Split(s_DirectorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new UmbrellaFileSystemException($"The value for {nameof(subpath)} must contain exactly 2 segments, i.e. folder name and file name. The folder name is used as the blob storage container name. The invalid value is {subpath}");

            string containerName = parts[0].ToLowerInvariant();
            string fileName = parts[1];

            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            //The call to ExistsAsync should force the properties of the blob to be populated
            if (!isNew && await blob.ExistsAsync().ConfigureAwait(false))
                throw new UmbrellaFileSystemException($"The blob for {nameof(subpath)} with value {subpath} does not exist.");

            return new UmbrellaAzureBlobFileInfo(LoggerFactory.CreateLogger<UmbrellaAzureBlobFileInfo>(), blob);
        }
    }
}