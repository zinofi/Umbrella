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
    public class UmbrellaAzureBlobFileProvider : IUmbrellaAzureBlobFileProvider
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

            string cleanedPath = subpath.Trim(' ', '~');

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

            return new UmbrellaAzureBlobFileInfo(LoggerFactory.CreateLogger<UmbrellaAzureBlobFileInfo>(), this, blob);
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

        public Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNull(fileInfo, nameof(fileInfo));

                return fileInfo.DeleteAsync(cancellationToken);
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

                return await CopyAsync(sourceFile, destinationSubpath, cancellationToken).ConfigureAwait(false);
            }
            catch(Exception exc) when (Log.WriteError(exc, new { sourceSubpath, destinationSubpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNull(sourceFile, nameof(sourceFile));
                Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

                IUmbrellaFileInfo destinationFile = await GetAsync(destinationSubpath, cancellationToken);

                if (destinationFile == null)
                    throw new UmbrellaFileNotFoundException(destinationSubpath);

                if (sourceFile is UmbrellaAzureBlobFileInfo blobSourceFile)
                {
                    var blobDestinationFile = (UmbrellaAzureBlobFileInfo)destinationFile;

                    await blobDestinationFile.Blob.StartCopyAsync(blobSourceFile.Blob);

                    return destinationFile;
                }
                else
                {
                    throw new Exception($"The {nameof(sourceFile)} parameter must be of type {nameof(UmbrellaAzureBlobFileInfo)}. This provider does currently not support copying files loaded using different provider implementations.");
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationSubpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
                Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

                IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
                await file.WriteFromByteArrayAsync(bytes, cancellationToken).ConfigureAwait(false);

                return file;
            }
            catch(Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<(bool Exists, IUmbrellaFileInfo FileInfo)> ExistsAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

                IUmbrellaFileInfo file = await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);

                return (file != null, file);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }
    }
}