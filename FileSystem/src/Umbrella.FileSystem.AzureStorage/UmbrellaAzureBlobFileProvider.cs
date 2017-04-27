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
    //TODO: Most of this could go into a base class when looking at the disk provider.
    public class UmbrellaAzureBlobFileProvider : IUmbrellaAzureBlobFileProvider
    {
        private static readonly char[] s_DirectorySeparatorArray = new[] { '/' };

        protected ILogger Log { get; }
        protected ILoggerFactory LoggerFactory { get; }
        protected IMimeTypeUtility MimeTypeUtility { get; }
        protected UmbrellaAzureBlobFileProviderOptions Options { get; }
        protected CloudStorageAccount StorageAccount { get; }
        protected CloudBlobClient BlobClient { get; }

        public UmbrellaAzureBlobFileProvider(ILoggerFactory loggerFactory,
            IMimeTypeUtility mimeTypeUtility,
            UmbrellaAzureBlobFileProviderOptions options)
        {
            LoggerFactory = loggerFactory;
            MimeTypeUtility = mimeTypeUtility;
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
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
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
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        private async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            StringBuilder pathBuilder = new StringBuilder(subpath)
                .Trim(' ')
                .Trim('~')
                .Trim('_')
                .Replace("_", "")
                .Replace(" ", "");

            string cleanedPath = pathBuilder.ToString();

            string[] parts = cleanedPath.Split(s_DirectorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new Exception($"The value for {nameof(subpath)} must contain exactly 2 segments, i.e. folder name and file name. The folder name is used as the blob storage container name. The invalid value is {subpath}");

            string containerName = parts[0].ToLowerInvariant();
            string fileName = parts[1];

            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            //The call to ExistsAsync should force the properties of the blob to be populated
            if (!isNew && !await blob.ExistsAsync().ConfigureAwait(false))
                return null;

            return new UmbrellaAzureBlobFileInfo(LoggerFactory.CreateLogger<UmbrellaAzureBlobFileInfo>(), MimeTypeUtility, this, blob, isNew);
        }

        public async Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

                IUmbrellaFileInfo fileInfo = await GetAsync(subpath, cancellationToken).ConfigureAwait(false);

                if (fileInfo != null)
                    return await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNull(fileInfo, nameof(fileInfo));

                return await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { fileInfo }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(sourceSubpath, nameof(sourceSubpath));
                Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

                IUmbrellaFileInfo sourceFile = await GetAsync(sourceSubpath, cancellationToken);

                if (sourceFile == null)
                    throw new UmbrellaFileNotFoundException(sourceSubpath);

                return await sourceFile.CopyAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { sourceSubpath, destinationSubpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentOfType<UmbrellaAzureBlobFileInfo>(sourceFile, nameof(sourceFile));
                Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

                IUmbrellaFileInfo destinationFile = await CreateAsync(destinationSubpath, cancellationToken);

                return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false); ;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationSubpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentOfType<UmbrellaAzureBlobFileInfo>(sourceFile, nameof(sourceFile));
                Guard.ArgumentOfType<UmbrellaAzureBlobFileInfo>(destinationFile, nameof(destinationFile));

                return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationFile }))
            {
                throw;
            }
        }

        public async Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
                Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

                IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
                await file.WriteFromByteArrayAsync(bytes, cacheContents, cancellationToken).ConfigureAwait(false);

                return file;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<bool> ExistsAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

                IUmbrellaFileInfo file = await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);

                return file != null;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }
    }
}