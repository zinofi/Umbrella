using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	/// <summary>
	/// An implementation of <see cref="IUmbrellaFileInfo"/> that uses Azure Blob Storage as the underlying storage mechanism.
	/// </summary>
	/// <seealso cref="IUmbrellaFileInfo" />
	/// <seealso cref="IEquatable{UmbrellaAzureBlobStorageFileInfo}" />
	public class UmbrellaAzureBlobStorageFileInfo : IUmbrellaFileInfo, IEquatable<UmbrellaAzureBlobStorageFileInfo>
	{
		#region Private Members
		private byte[]? _content;
		private long _length = -1;
		private string? _contentType;
		private BlobProperties? _blobProperties;
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the file provider used to create this file.
		/// </summary>
		protected IUmbrellaAzureBlobStorageFileProvider Provider { get; }

		/// <summary>
		/// Gets the generic type converter.
		/// </summary>
		protected IGenericTypeConverter GenericTypeConverter { get; }
		#endregion

		#region Internal Properties
		internal BlobClient Blob { get; }
		#endregion

		#region Public Properties
		/// <inheritdoc />
		public bool IsNew { get; private set; }

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public string SubPath { get; }

		/// <inheritdoc />
		public long Length
		{
			get => IsNew ? _length : _blobProperties?.ContentLength ?? -1;
			private set => _length = value;
		}

		/// <inheritdoc />
		public DateTimeOffset? LastModified => IsNew ? null : _blobProperties?.LastModified;

		/// <inheritdoc />
		public string? ContentType
		{
			get => IsNew ? _contentType : _blobProperties?.ContentType;
			set => _contentType = value;
		}
		#endregion

		#region Constructors
		internal UmbrellaAzureBlobStorageFileInfo(ILogger<UmbrellaAzureBlobStorageFileInfo> logger,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			string subpath,
			IUmbrellaAzureBlobStorageFileProvider provider,
			BlobClient blob,
			bool isNew)
		{
			Logger = logger;
			Provider = provider;
			GenericTypeConverter = genericTypeConverter;

			Blob = blob;
			IsNew = isNew;
			SubPath = subpath;
			Name = Path.GetFileName(subpath);

			ContentType = mimeTypeUtility.GetMimeType(Name);
		}
		#endregion

		#region Internal Methods
		internal async Task InitializeAsync(CancellationToken cancellationToken)
		{
			// We can only get the properties for a Blob if it exists. The method call will throw
			// an exception otherwise.
			if (IsNew)
			{
				_blobProperties = new BlobProperties();
				return;
			}

			try
			{
				var result = await Blob.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

				if (result != null)
					_blobProperties = result.Value;
			}
			catch (RequestFailedException exc) when (Logger.WriteWarning(exc))
			{
				throw;
			}
		}
		#endregion

		#region IUmbrellaFileInfo Members
		/// <inheritdoc />
		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem deleting the file.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
			}
			catch(RequestFailedException exc) when (exc.ErrorCode == BlobErrorCode.ContainerBeingDeleted)
			{
				// The container is in the process of being deleted which is fine and means the Blob is on its way to Blob heaven.
				return false;
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem determining if the file exists.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				if (cacheContents && _content != null)
					return _content;

				if (!await ExistsAsync(cancellationToken).ConfigureAwait(false))
					throw new UmbrellaFileNotFoundException(SubPath);

				using Stream stream = await ReadAsStreamAsync(cancellationToken, bufferSizeOverride).ConfigureAwait(false);
				using var ms = new MemoryStream();

				await stream.CopyToAsync(ms, bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize).ConfigureAwait(false);

				byte[] bytes = ms.ToArray();

				_content = cacheContents ? bytes : null;

				return bytes;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem reading the file to a byte array.", exc);
			}
		}

		/// <inheritdoc />
		public async Task WriteToStreamAsync(Stream target, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNull(target, nameof(target));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				await Blob.DownloadToAsync(target, transferOptions: CreateStorageTransferOptions(bufferSizeOverride), cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem writing the file to the specified stream.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				using var ms = new MemoryStream(bytes);
				await WriteFromStreamAsync(ms, cancellationToken, bufferSizeOverride).ConfigureAwait(false);

				_content = cacheContents ? bytes : null;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem writing to the file from the specified bytes.", exc);
			}
		}

		/// <inheritdoc />
		public async Task WriteFromStreamAsync(Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				stream.Position = 0;
				
				await Blob.UploadAsync(
					stream,
					new BlobHttpHeaders { ContentType = ContentType },
					_blobProperties?.Metadata,
					transferOptions: CreateStorageTransferOptions(bufferSizeOverride),
					cancellationToken: cancellationToken).ConfigureAwait(false);

				IsNew = false;

				// Trigger a call to this to ensure property population
				await InitializeAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem writing to the file from the specified stream.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				if (!await ExistsAsync(cancellationToken).ConfigureAwait(false))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaAzureBlobStorageFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
				await CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);

				return destinationFile;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem copying the file to the specified destination path.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<UmbrellaAzureBlobStorageFileInfo>(destinationFile, nameof(destinationFile), "Copying files between providers of different types is not supported.");

			try
			{
				if (!await ExistsAsync(cancellationToken).ConfigureAwait(false))
					throw new UmbrellaFileNotFoundException(SubPath);

				var blobDestinationFile = (UmbrellaAzureBlobStorageFileInfo)destinationFile;

				await blobDestinationFile.Blob.StartCopyFromUriAsync(Blob.Uri, cancellationToken: cancellationToken).ConfigureAwait(false);

				// In order to ensure we know the size of the destination file we can set it here
				// and then use the real value from the Blob once it becomes available after the copy operation
				// has completed.
				blobDestinationFile.Length = Length;
				blobDestinationFile.IsNew = false;

				await blobDestinationFile.InitializeAsync(cancellationToken).ConfigureAwait(false);

				return destinationFile;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { destinationFile }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem copying the file to the specified destination file.", exc);
			}
		}

		/// <inheritdoc />
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
			catch (Exception exc) when (Logger.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem moving the file to the specified destination path.", exc);
			}
		}

		/// <inheritdoc />
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
			catch (Exception exc) when (Logger.WriteError(exc, new { destinationFile }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem moving the specified file to the specified destination file.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				// The buffer size can't be controlled here. For chunking we should use the WriteToStreamAsync method.
				BlobDownloadInfo response = await Blob.DownloadAsync(cancellationToken).ConfigureAwait(false);

				return response.Content;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error reading the Blob as a Stream.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<T> GetMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, T fallback = default!, Func<string?, T>? customValueConverter = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				if (_blobProperties != null)
				{
					_blobProperties.Metadata.TryGetValue(key, out string rawValue);

					return await Task.FromResult(GenericTypeConverter.Convert(rawValue, fallback, customValueConverter)!).ConfigureAwait(false);
				}

				return default!;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { key, fallback, customValueConverter }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error getting the metadata value for the specified key.", exc);
			}
		}

		/// <inheritdoc />
		public async Task SetMetadataValueAsync<T>(string key, T value, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				if (_blobProperties != null)
				{
					if (value is null)
					{
						_blobProperties.Metadata.Remove(key);
					}
					else
					{
						_blobProperties.Metadata[key] = value.ToString();
					}

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { key, value, writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error setting the metadata value for the specified key.", exc);
			}
		}

		/// <inheritdoc />
		public async Task RemoveMetadataValueAsync(string key, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				if (_blobProperties != null)
				{
					_blobProperties.Metadata.Remove(key);

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { key, writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error removing the metadata value for the specified key.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ClearMetadataAsync(CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				if (_blobProperties != null)
				{
					_blobProperties.Metadata.Clear();

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { writeChanges }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error clearing the metadata.", exc);
			}
		}

		/// <inheritdoc />
		public async Task WriteMetadataChangesAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				if (_blobProperties != null)
					await Blob.SetMetadataAsync(_blobProperties.Metadata, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error writing the metadata changes.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<TUserId> GetCreatedByIdAsync<TUserId>(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await GetMetadataValueAsync<TUserId>(UmbrellaFileSystemConstants.CreatedByIdMetadataKey, cancellationToken);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error getting the id.", exc);
			}
		}

		/// <inheritdoc />
		public async Task SetCreatedByIdAsync<TUserId>(TUserId value, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				await SetMetadataValueAsync(UmbrellaFileSystemConstants.CreatedByIdMetadataKey, value, cancellationToken, writeChanges);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error setting the id.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<string> GetFileNameAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await GetMetadataValueAsync<string>(UmbrellaFileSystemConstants.FileNameMetadataKey, cancellationToken);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error getting the file name.", exc);
			}
		}

		/// <inheritdoc />
		public async Task SetFileNameAsync(string value, CancellationToken cancellationToken = default, bool writeChanges = true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				await SetMetadataValueAsync(UmbrellaFileSystemConstants.FileNameMetadataKey, value, cancellationToken, writeChanges);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error setting the file name.", exc);
			}
		}
		#endregion

		#region Private Methods
		private StorageTransferOptions CreateStorageTransferOptions(int? bufferSizeOverride = null)
			=> new StorageTransferOptions { MaximumTransferLength = bufferSizeOverride ?? UmbrellaFileSystemConstants.LargeBufferSize };

		private void ThrowIfIsNew()
		{
			if (IsNew)
				throw new InvalidOperationException("Cannot perform this operation on a newly created file. The file must first be written to.");
		}
		#endregion

		#region IEquatable Members
		/// <inheritdoc />
		public bool Equals(UmbrellaAzureBlobStorageFileInfo? other)
			=> !(other is null) &&
				IsNew == other.IsNew &&
				Name == other.Name &&
				SubPath == other.SubPath &&
				Length == other.Length &&
				EqualityComparer<DateTimeOffset?>.Default.Equals(LastModified, other.LastModified) &&
				ContentType == other.ContentType;
		#endregion

		#region Overridden Methods
		/// <inheritdoc />
		public override bool Equals(object obj) => Equals(obj as UmbrellaAzureBlobStorageFileInfo);

		/// <inheritdoc />
		public override int GetHashCode()
		{
			int hashCode = 260482354;
			hashCode = hashCode * -1521134295 + IsNew.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SubPath);
			hashCode = hashCode * -1521134295 + Length.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset?>.Default.GetHashCode(LastModified);
			
			if(ContentType != null)
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContentType);

			return hashCode;
		}
		#endregion

		#region Operators
		/// <inheritdoc />
		public static bool operator ==(UmbrellaAzureBlobStorageFileInfo left, UmbrellaAzureBlobStorageFileInfo right) => EqualityComparer<UmbrellaAzureBlobStorageFileInfo>.Default.Equals(left, right);

		/// <inheritdoc />
		public static bool operator !=(UmbrellaAzureBlobStorageFileInfo left, UmbrellaAzureBlobStorageFileInfo right) => !(left == right);
		#endregion
	}
}