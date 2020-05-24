using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage.Extensions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	public class UmbrellaAzureBlobStorageFileInfo : IUmbrellaFileInfo, IEquatable<UmbrellaAzureBlobStorageFileInfo>
	{
		#region Private Members
		private byte[] m_Contents;
		private long m_Length = -1;
		#endregion

		#region Protected Properties
		protected ILogger Log { get; }
		protected IUmbrellaAzureBlobStorageFileProvider Provider { get; }
		protected IGenericTypeConverter GenericTypeConverter { get; }
		#endregion

		#region Internal Properties
		internal CloudBlockBlob Blob { get; }
		#endregion

		#region Public Properties
		public bool IsNew { get; private set; }
		public string Name { get; }
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
			IUmbrellaAzureBlobStorageFileProvider provider,
			CloudBlockBlob blob,
			bool isNew)
		{
			Log = logger;
			Provider = provider;
			GenericTypeConverter = genericTypeConverter;

			Blob = blob;
			IsNew = isNew;
			SubPath = subpath;
			Name = Path.GetFileName(subpath);

			ContentType = mimeTypeUtility.GetMimeType(Name);
		}
		#endregion

		#region IUmbrellaFileInfo Members
		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				if (cacheContents && m_Contents != null)
					return m_Contents;

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				byte[] bytes = new byte[Blob.Properties.Length];

				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;

				await Blob.DownloadToByteArrayAsync(bytes, 0, cancellationToken).ConfigureAwait(false);

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
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNull(target, nameof(target));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;

				await Blob.DownloadToStreamAsync(target, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;

				await Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);

				//Trigger a call to this to ensure property population
				await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);

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
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				stream.Position = 0;
				Blob.StreamMinimumReadSizeInBytes = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize;

				await Blob.UploadFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

				//Trigger a call to this to ensure property population
				await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);

				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaAzureBlobStorageFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				await destinationFile.Blob.StartCopyAsync(Blob, cancellationToken).ConfigureAwait(false);
				
				// In order to ensure we know the size of the destination file we can set it here
				// and then use the real value from the Blob once it becomes available after the copy operation
				// has completed.
				destinationFile.Length = Length;
				destinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<UmbrellaAzureBlobStorageFileInfo>(destinationFile, nameof(destinationFile), "Copying files between providers of different types is not supported.");

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var blobDestinationFile = (UmbrellaAzureBlobStorageFileInfo)destinationFile;

				await blobDestinationFile.Blob.StartCopyAsync(Blob, cancellationToken).ConfigureAwait(false);

				// In order to ensure we know the size of the destination file we can set it here
				// and then use the real value from the Blob once it becomes available after the copy operation
				// has completed.
				blobDestinationFile.Length = Length;
				blobDestinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> MoveAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				IUmbrellaFileInfo destinationFile = await CopyAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
				await DeleteAsync(cancellationToken).ConfigureAwait(false);

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> MoveAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<UmbrellaAzureBlobStorageFileInfo>(destinationFile, nameof(destinationFile), "Moving files between providers of different types is not supported.");

			try
			{
				await CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
				await DeleteAsync(cancellationToken).ConfigureAwait(false);

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true))
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
				
				return await Blob.OpenReadAsync(cancellationToken).ConfigureAwait(false);
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
					await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
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
					await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error removing the metadata value for the specified key.", exc);
			}
		}

		public async Task ClearMetadataAsync<T>(CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				Blob.Metadata.Clear();

				if (writeChanges)
					await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error clearing the metadata.", exc);
			}
		}

		public async Task WriteMetadataChangesAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				await Blob.SetMetadataAsync(cancellationToken).ConfigureAwait(false);
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

		#region IEquatable Members
		public bool Equals(UmbrellaAzureBlobStorageFileInfo other)
			=> other != null &&
				IsNew == other.IsNew &&
				Name == other.Name &&
				SubPath == other.SubPath &&
				Length == other.Length &&
				EqualityComparer<DateTimeOffset?>.Default.Equals(LastModified, other.LastModified) &&
				ContentType == other.ContentType;
		#endregion

		#region Overridden Methods
		public override bool Equals(object obj) => Equals(obj as UmbrellaAzureBlobStorageFileInfo);

		public override int GetHashCode()
		{
			int hashCode = 260482354;
			hashCode = hashCode * -1521134295 + IsNew.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SubPath);
			hashCode = hashCode * -1521134295 + Length.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset?>.Default.GetHashCode(LastModified);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContentType);
			return hashCode;
		}
		#endregion

		#region Operators
		public static bool operator ==(UmbrellaAzureBlobStorageFileInfo left, UmbrellaAzureBlobStorageFileInfo right) => EqualityComparer<UmbrellaAzureBlobStorageFileInfo>.Default.Equals(left, right);

		public static bool operator !=(UmbrellaAzureBlobStorageFileInfo left, UmbrellaAzureBlobStorageFileInfo right) => !(left == right);
		#endregion
	}
}