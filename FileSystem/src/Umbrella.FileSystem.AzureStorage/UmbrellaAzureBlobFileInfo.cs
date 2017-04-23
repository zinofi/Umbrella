using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
    public class UmbrellaAzureBlobFileInfo : IUmbrellaFileInfo
    {
        private byte[] m_Contents;

        protected ILogger Log { get; }
        internal CloudBlockBlob Blob { get; }
        protected IUmbrellaFileProvider Provider { get; }

        public string Name => Blob.Name;
        public long Length => Blob.Properties.Length;
        public DateTimeOffset? LastModified => Blob.Properties.LastModified;

        public UmbrellaAzureBlobFileInfo(ILogger<UmbrellaAzureBlobFileInfo> logger, IUmbrellaFileProvider provider, CloudBlockBlob blob)
        {
            Log = Log;
            Provider = provider;
            Blob = blob;
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

                if (cacheContents && m_Contents != null)
                    return m_Contents;

                byte[] bytes = new byte[Blob.Properties.Length];
                await Blob.DownloadToByteArrayAsync(bytes, 0).ConfigureAwait(false);

                if (cacheContents)
                    m_Contents = bytes;

                return bytes;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { cacheContents }))
            {
                throw;
            }
        }

        public async Task WriteFromByteArrayAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken)
        {
            try
            {
                return await Provider.CopyAsync(this, destinationSubpath, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }))
            {
                throw;
            }
        }
    }
}