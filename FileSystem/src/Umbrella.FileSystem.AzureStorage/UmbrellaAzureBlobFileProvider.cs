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
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime;

namespace Umbrella.FileSystem.AzureStorage
{
    public class UmbrellaAzureBlobFileProvider : UmbrellaFileProvider<UmbrellaAzureBlobFileInfo, UmbrellaAzureBlobFileProviderOptions>, IUmbrellaAzureBlobFileProvider
    {
        private static readonly char[] s_DirectorySeparatorArray = new[] { '/', '\\' };

        protected CloudStorageAccount StorageAccount { get; }
        protected CloudBlobClient BlobClient { get; }

        public UmbrellaAzureBlobFileProvider(ILoggerFactory loggerFactory,
            IMimeTypeUtility mimeTypeUtility,
            UmbrellaAzureBlobFileProviderOptions options)
            : base(loggerFactory.CreateLogger<UmbrellaAzureBlobFileProvider>(), loggerFactory, mimeTypeUtility, options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(options.StorageConnectionString, nameof(options.StorageConnectionString));

            StorageAccount = CloudStorageAccount.Parse(options.StorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //TODO: Use a regex here to only allow the characters permitted by Azure which are:
            //Length: between 3 and 63 chars
            //Lowercase letters
            //Numbers
            //Hyphens
            //Can't contain consecutive hyphens
            //Begin and end with a letter or number
            StringBuilder pathBuilder = new StringBuilder(subpath)
                .Trim(' ', '\\', '/', '~', '_', ' ')
                .Replace("_", "")
                .Replace(" ", "");

            string cleanedPath = pathBuilder.ToString();

            if(Log.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                Log.WriteDebug(new { subpath, cleanedPath });

            string[] parts = cleanedPath.Split(s_DirectorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

            //TODO: Should extend this to allow for nested folder structures
            if (parts.Length != 2)
                throw new UmbrellaFileSystemException($"The value for {nameof(subpath)} must contain exactly 2 segments, i.e. folder name and file name. The folder name is used as the blob storage container name. The invalid value is {subpath}");

            string containerName = parts[0].ToLowerInvariant();
            string fileName = parts[1];

            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
#if net46
            await container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
#else
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
#endif

            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            //The call to ExistsAsync should force the properties of the blob to be populated
#if net46
            if (!isNew && !await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
                return null;
#else
            if (!isNew && !await blob.ExistsAsync().ConfigureAwait(false))
                return null;
#endif

            return new UmbrellaAzureBlobFileInfo(FileInfoLoggerInstance, MimeTypeUtility, subpath, this, blob, isNew);
        }
    }
}