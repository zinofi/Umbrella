using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Disk
{
	/// <summary>
	/// An implementation of <see cref="IUmbrellaFileInfo"/> that uses the physical disk as the underlying storage mechanism.
	/// </summary>
	/// <seealso cref="IUmbrellaFileInfo" />
	/// <seealso cref="IEquatable{UmbrellaDiskFileInfo}" />
	public class UmbrellaDiskFileInfo : IUmbrellaFileInfo, IEquatable<UmbrellaDiskFileInfo>
	{
		#region Private Members
		private readonly string _metadataFullFileName;
		private byte[]? m_Contents;
		private Dictionary<string, string>? _metadataDictionary;
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the <see cref="ILogger" />.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the provider used to load this file instance.
		/// </summary>
		protected IUmbrellaDiskFileProvider Provider { get; }

		/// <summary>
		/// Gets the generic type converter.
		/// </summary>
		protected IGenericTypeConverter GenericTypeConverter { get; }
		#endregion

		#region Internal Properties
		internal FileInfo PhysicalFileInfo { get; }
		#endregion

		#region Public Properties
		/// <inheritdoc />
		public bool IsNew { get; private set; }
		
		/// <inheritdoc />
		public string Name => PhysicalFileInfo.Name;

		/// <inheritdoc />
		public string SubPath { get; }

		/// <inheritdoc />
		public long Length => PhysicalFileInfo.Exists && !IsNew ? PhysicalFileInfo.Length : -1;

		/// <inheritdoc />
		public DateTimeOffset? LastModified => PhysicalFileInfo.Exists && !IsNew ? PhysicalFileInfo.LastWriteTimeUtc : (DateTimeOffset?)null;

		/// <inheritdoc />
		public string ContentType { get; }
		#endregion

		#region Constructors
		internal UmbrellaDiskFileInfo(
			ILogger<UmbrellaDiskFileInfo> logger,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			string subpath,
			IUmbrellaDiskFileProvider provider,
			FileInfo physicalFileInfo,
			bool isNew)
		{
			if (subpath.EndsWith(UmbrellaDiskFileConstants.MetadataFileExtension, StringComparison.OrdinalIgnoreCase))
				throw new UmbrellaFileSystemException($"Files with the extension '{UmbrellaDiskFileConstants.MetadataFileExtension}' are not permitted.");

			Log = logger;
			Provider = provider;
			GenericTypeConverter = genericTypeConverter;
			PhysicalFileInfo = physicalFileInfo;
			IsNew = isNew;
			SubPath = subpath;

			ContentType = mimeTypeUtility.GetMimeType(Name);

			_metadataFullFileName = PhysicalFileInfo.FullName + UmbrellaDiskFileConstants.MetadataFileExtension;
		}
		#endregion

		#region IUmbrellaFileInfo Members
		/// <inheritdoc />
		public async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaDiskFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				if (!destinationFile.PhysicalFileInfo.Directory.Exists)
					destinationFile.PhysicalFileInfo.Directory.Create();

				File.Copy(PhysicalFileInfo.FullName, destinationFile.PhysicalFileInfo.FullName, true);

				if (File.Exists(_metadataFullFileName))
					File.Copy(_metadataFullFileName, destinationFile.PhysicalFileInfo.FullName + UmbrellaDiskFileConstants.MetadataFileExtension, true);

				destinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem copying the current file to the specified destination.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<UmbrellaDiskFileInfo>(destinationFile, nameof(destinationFile), "Copying files between providers of different types is not supported.");

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var target = (UmbrellaDiskFileInfo)destinationFile;

				if (!target.PhysicalFileInfo.Directory.Exists)
					target.PhysicalFileInfo.Directory.Create();

				File.Copy(PhysicalFileInfo.FullName, target.PhysicalFileInfo.FullName, true);

				if (File.Exists(_metadataFullFileName))
					File.Copy(_metadataFullFileName, target.PhysicalFileInfo.FullName + UmbrellaDiskFileConstants.MetadataFileExtension, true);

				target.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem copying the current file to the specified destination.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<IUmbrellaFileInfo> MoveAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaDiskFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				if (!destinationFile.PhysicalFileInfo.Directory.Exists)
					destinationFile.PhysicalFileInfo.Directory.Create();

				File.Move(PhysicalFileInfo.FullName, destinationFile.PhysicalFileInfo.FullName);

				if (File.Exists(_metadataFullFileName))
					File.Move(_metadataFullFileName, destinationFile.PhysicalFileInfo.FullName + UmbrellaDiskFileConstants.MetadataFileExtension);

				destinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem moving the current file to the specified destination.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<IUmbrellaFileInfo> MoveAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<UmbrellaDiskFileInfo>(destinationFile, nameof(destinationFile), "Moving files between providers of different types is not supported.");

			try
			{
				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var target = (UmbrellaDiskFileInfo)destinationFile;

				if (!target.PhysicalFileInfo.Directory.Exists)
					target.PhysicalFileInfo.Directory.Create();

				File.Move(PhysicalFileInfo.FullName, target.PhysicalFileInfo.FullName);

				if (File.Exists(_metadataFullFileName))
					File.Move(_metadataFullFileName, target.PhysicalFileInfo.FullName + UmbrellaDiskFileConstants.MetadataFileExtension);

				target.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem moving the current file to the specified destination.", exc);
			}
		}

		/// <inheritdoc />
		public Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				PhysicalFileInfo.Delete();
				File.Delete(_metadataFullFileName);

				return Task.FromResult(true);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem deleting the current file.", exc);
			}
		}

		/// <inheritdoc />
		public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return Task.FromResult(PhysicalFileInfo.Exists);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There was a problem determining if the current file exists.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				if (cacheContents && m_Contents != null)
					return m_Contents;

				byte[] bytes = new byte[PhysicalFileInfo.Length];

				using (var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize, true))
				{
					await fs.ReadAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
				}

				m_Contents = cacheContents ? bytes : null;

				return bytes;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
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
				int bufferSize = bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize;

				using var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true);
				await fs.CopyToAsync(target, bufferSize, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));
			Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

			try
			{
				if (!PhysicalFileInfo.Directory.Exists)
					PhysicalFileInfo.Directory.Create();

				using (var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write, bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize, true))
				{
					await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
				}

				m_Contents = cacheContents ? bytes : null;
				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { cacheContents, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
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
				if (!PhysicalFileInfo.Directory.Exists)
					PhysicalFileInfo.Directory.Create();

				int bufferSize = bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize;

				using (var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write, bufferSize, true))
				{
					stream.Position = 0;
					await stream.CopyToAsync(fs, bufferSize, cancellationToken).ConfigureAwait(false);
				}

				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
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
				return await Task.FromResult(new FileStream(PhysicalFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize, true)).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public async Task<T> GetMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, T fallback = default, Func<string?, T>? customValueConverter = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				if (_metadataDictionary is null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				if (_metadataDictionary != null)
				{
					_metadataDictionary.TryGetValue(key, out string rawValue);

					return GenericTypeConverter.Convert(rawValue, fallback, customValueConverter)!;
				}

				return default!;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, fallback, customValueConverter }, returnValue: true))
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
				if (_metadataDictionary is null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				if (_metadataDictionary != null)
				{
					if (value is null)
					{
						_metadataDictionary.Remove(key);
					}
					else
					{
						_metadataDictionary[key] = value.ToString();
					}

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, value, writeChanges }, returnValue: true))
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
				if (_metadataDictionary is null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				if (_metadataDictionary != null)
				{
					_metadataDictionary.Remove(key);

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { key, writeChanges }, returnValue: true))
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
				if (_metadataDictionary is null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				if (_metadataDictionary != null)
				{
					_metadataDictionary.Clear();

					if (writeChanges)
						await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { writeChanges }, returnValue: true))
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
				if (_metadataDictionary?.Count > 0)
				{
					string json = UmbrellaStatics.SerializeJson(_metadataDictionary);

					using var fs = new FileStream(_metadataFullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);
					using var sr = new StreamWriter(fs);

					await sr.WriteAsync(json).ConfigureAwait(false);
				}
				else
				{
					// Before deleting the file, reload the metadata just in case we have some on disk that hasn't be loaded into memory.
					await ReloadMetadataAsync(cancellationToken);

					if(_metadataDictionary == null || _metadataDictionary.Count == 0)
						File.Delete(_metadataFullFileName);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
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
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
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
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
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
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
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
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error setting the file name.", exc);
			}
		}
		#endregion

		#region IEquatable Members		
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(UmbrellaDiskFileInfo? other)
			=> !(other is null) &&
				IsNew == other.IsNew &&
				Name == other.Name &&
				SubPath == other.SubPath &&
				Length == other.Length &&
				EqualityComparer<DateTimeOffset?>.Default.Equals(LastModified, other.LastModified) &&
				ContentType == other.ContentType;
		#endregion

		#region Overridden Methods		
		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) => Equals(obj as UmbrellaDiskFileInfo);

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
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
		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(UmbrellaDiskFileInfo left, UmbrellaDiskFileInfo right) => EqualityComparer<UmbrellaDiskFileInfo>.Default.Equals(left, right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(UmbrellaDiskFileInfo left, UmbrellaDiskFileInfo right) => !(left == right);
		#endregion

		#region Private Methods
		private async Task ReloadMetadataAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				string? json = null;

				if (!File.Exists(_metadataFullFileName))
				{
					_metadataDictionary = new Dictionary<string, string>();
					return;
				}

				using (var fs = new FileStream(_metadataFullFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
				{
					using var sr = new StreamReader(fs);
					json = await sr.ReadToEndAsync().ConfigureAwait(false);
				}

				if (string.IsNullOrWhiteSpace(json))
				{
					_metadataDictionary = new Dictionary<string, string>();
					return;
				}

				try
				{
					_metadataDictionary = UmbrellaStatics.DeserializeJson<Dictionary<string, string>>(json);
				}
				catch (Exception exc) when (Log.WriteError(exc, new { json }, "The JSON value stored in the metadata file could not be deserialized to a Dictionary. This error has been handled silently.", returnValue: true))
				{
					_metadataDictionary = new Dictionary<string, string>();
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error reloading the metadata for the file.", exc);
			}
		}

		private void ThrowIfIsNew()
		{
			if (IsNew)
				throw new InvalidOperationException("Cannot read the contents of a newly created file. The file must first be written to.");
		}
		#endregion
	}
}