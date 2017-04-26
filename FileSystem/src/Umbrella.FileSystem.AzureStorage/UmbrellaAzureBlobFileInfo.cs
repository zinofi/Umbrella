using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime;

namespace Umbrella.FileSystem.AzureStorage
{
    public class UmbrellaAzureBlobFileInfo : IUmbrellaFileInfo
    {
        private byte[] m_Contents;

        protected ILogger Log { get; }
        protected UmbrellaAzureBlobFileProvider Provider { get; }

        internal CloudBlockBlob Blob { get; }

        public bool IsNew { get; private set; }
        public string Name => Blob.Name;
        public long Length => Blob.Properties.Length;
        public DateTimeOffset? LastModified => Blob.Properties.LastModified;
        public string ContentType
        {
            get => Blob.Properties.ContentType;
            private set => Blob.Properties.ContentType = value;
        }

        public UmbrellaAzureBlobFileInfo(ILogger<UmbrellaAzureBlobFileInfo> logger,
            IMimeTypeUtility mimeTypeUtility,
            UmbrellaAzureBlobFileProvider provider,
            CloudBlockBlob blob,
            bool isNew)
        {
            Log = logger;
            Provider = provider;
            Blob = blob;
            IsNew = isNew;

            ContentType = mimeTypeUtility.GetMimeType(Name);
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await Blob.DeleteIfExistsAsync().ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await Blob.ExistsAsync().ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default(CancellationToken), bool cacheContents = true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsNew)
                    throw new InvalidOperationException("Cannot read the contents of a newly created file. The file must first be written to.");

                if (cacheContents && m_Contents != null)
                    return m_Contents;

                byte[] bytes = new byte[Blob.Properties.Length];
                await Blob.DownloadToByteArrayAsync(bytes, 0).ConfigureAwait(false);

                m_Contents = cacheContents ? bytes : null;

                return bytes;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheContents }))
            {
                throw;
            }
        }

        public async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

                await Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

                m_Contents = cacheContents ? bytes : null;
                IsNew = false;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

                var destinationFile = (UmbrellaAzureBlobFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
                await destinationFile.Blob.StartCopyAsync(Blob).ConfigureAwait(false);

                return destinationFile;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentOfType<UmbrellaAzureBlobFileInfo>(destinationFile, nameof(destinationFile));

                var blobDestinationFile = (UmbrellaAzureBlobFileInfo)destinationFile;
                await blobDestinationFile.Blob.StartCopyAsync(Blob).ConfigureAwait(false);

                return destinationFile;
            }
            catch(Exception exc) when (Log.WriteError(exc, new { destinationFile }))
            {
                throw new UmbrellaFileSystemException(exc.Message, exc);
            }
        }
    }
}