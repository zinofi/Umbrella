using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	//TODO: Override Equals, GetHashCode, etc to allow for equality comparisons
	public class UmbrellaAzureBlobStorageFileInfo : IUmbrellaFileInfo
	{
		#region Private Members
		private byte[] m_Contents;
		private long m_Length = -1;
		#endregion

		#region Protected Properties
		protected ILogger Log { get; }
		protected UmbrellaAzureBlobStorageFileProvider Provider { get; }
		protected IGenericTypeConverter GenericTypeConverter { get; }
		#endregion

		#region Internal Properties
		internal CloudBlockBlob Blob { get; }
		#endregion

		#region Public Properties
		public bool IsNew { get; private set; }
		public string Name => Blob.Name;
		public string SubPath { get; }
		public long Length
		{
			get => Blob.Properties.Length > -1 ? Blob.Properties.Length : m_Length;
			private set => m_Length = value;
		}
		public DateTimeOffset? LastModified => Blob.Properties.LastModified;
		public string ContentType
		{
			get => Blob.Properties.ContentType;
			private set => Blob.Properties.ContentType = value;
		}
		#endregion

		#region Constructors
		internal UmbrellaAzureBlobStorageFileInfo(ILogger<UmbrellaAzureBlobStorageFileInfo> logger,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			string subpath,
			UmbrellaAzureBlobStorageFileProvider provider,
			CloudBlockBlob blob,
			bool isNew)
		{
			Log = logger;
			Provider = provider;
			GenericTypeConverter = genericTypeConverter;

			Blob = blob;
			IsNew = isNew;
			SubPath = subpath;

			ContentType = mimeTypeUtility.GetMimeType(Name);
		}
		#endregion

		#region IUmbrellaFileInfo Members
		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
#if NET461
				return await Blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
#else
				return await Blob.DeleteIfExistsAsync().ConfigureAwait(false);
#endif
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

#if NET461
				return await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
#else
				return await Blob.ExistsAsync().ConfigureAwait(false);
#endif
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfIsNew();
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				if (cacheContents && m_Contents != null)
					return m_Contents;

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				byte[] bytes = new byte[Blob.Properties.Length];

				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;
#if NET461
				await Blob.DownloadToByteArrayAsync(bytes, 0, cancellationToken).ConfigureAwait(false);
#else
				await Blob.DownloadToByteArrayAsync(bytes, 0).ConfigureAwait(false);
#endif

				m_Contents = cacheContents ? bytes : null;

				return bytes;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task WriteToStreamAsync(Stream target, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfIsNew();
				Guard.ArgumentNotNull(target, nameof(target));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;
#if NET461
				await Blob.DownloadToStreamAsync(target, cancellationToken).ConfigureAwait(false);
#else
				await Blob.DownloadToStreamAsync(target).ConfigureAwait(false);
#endif
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;
#if NET461
				await Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
#else
				await Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
#endif

				//Trigger a call to this to ensure property population
#if NET461
				await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
#else
				await Blob.ExistsAsync().ConfigureAwait(false);
#endif

				m_Contents = cacheContents ? bytes : null;
				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task WriteFromStreamAsync(Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNull(stream, nameof(stream));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;
#if NET461
				await Blob.UploadFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);
#else
				await Blob.UploadFromStreamAsync(stream).ConfigureAwait(false);
#endif

				//Trigger a call to this to ensure property population
#if NET461
				await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
#else
				await Blob.ExistsAsync().ConfigureAwait(false);
#endif

				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaAzureBlobStorageFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
#if NET461
				await destinationFile.Blob.StartCopyAsync(Blob, cancellationToken).ConfigureAwait(false);
#else
				await destinationFile.Blob.StartCopyAsync(Blob).ConfigureAwait(false);
#endif

				//In order to ensure we know the size of the destination file we can set it here
				//and then use the real value from the Blob once it becomes available after the copy operation
				//has completed.
				destinationFile.Length = Length;
				destinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentOfType<UmbrellaAzureBlobStorageFileInfo>(destinationFile, nameof(destinationFile), "Copying files between providers of different types is not supported.");

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var blobDestinationFile = (UmbrellaAzureBlobStorageFileInfo)destinationFile;
#if NET461
				await blobDestinationFile.Blob.StartCopyAsync(Blob, cancellationToken).ConfigureAwait(false);
#else
				await blobDestinationFile.Blob.StartCopyAsync(Blob).ConfigureAwait(false);
#endif

				//In order to ensure we know the size of the destination file we can set it here
				//and then use the real value from the Blob once it becomes available after the copy operation
				//has completed.
				blobDestinationFile.Length = Length;
				blobDestinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;
#if NET461
				return await Blob.OpenReadAsync(cancellationToken).ConfigureAwait(false);
#else
				return await Blob.OpenReadAsync().ConfigureAwait(false);
#endif
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task<T> GetMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, T fallback = default, Func<string, T> customValueConverter = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				Blob.Metadata.TryGetValue(key, out string rawValue);

				return await Task.FromResult(GenericTypeConverter.Convert(rawValue, fallback, customValueConverter)).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, fallback, customValueConverter }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error getting the metadata value for the specified key.", exc);
			}
		}

		public async Task SetMetadataValueAsync<T>(string key, T value, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				if (value == null)
				{
					Blob.Metadata.Remove(key);
				}
				else
				{
					Blob.Metadata[key] = value.ToString();
				}

				if (writeChanges)
					await WriteMetadataChangesAsync().ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, value, writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error setting the metadata value for the specified key.", exc);
			}
		}

		public async Task RemoveMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				Blob.Metadata.Remove(key);

				if (writeChanges)
					await WriteMetadataChangesAsync().ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error removing the metadata value for the specified key.", exc);
			}
		}

		public async Task WriteMetadataChangesAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
#if NET461
				await Blob.SetMetadataAsync(cancellationToken).ConfigureAwait(false);
#else
				await Blob.SetMetadataAsync().ConfigureAwait(false);
#endif
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error writing the metadata changes.", exc);
			}
		}
		#endregion

		#region Private Methods
		private void ThrowIfIsNew()
		{
			if (IsNew)
				throw new InvalidOperationException("Cannot read the contents of a newly created file. The file must first be written to.");
		}
		#endregion
	}
}