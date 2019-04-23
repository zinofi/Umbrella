using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Disk
{
	//TODO: Override Equals, GetHashCode, etc to allow for equality comparisons
	public class UmbrellaDiskFileInfo : IUmbrellaFileInfo
	{
		#region Private Members
		private readonly string _metadataFullFileName;
		private byte[] m_Contents;
		private Dictionary<string, string> _metadataDictionary;
		#endregion

		#region Protected Properties
		protected ILogger Log { get; }
		protected UmbrellaDiskFileProvider Provider { get; }
		protected IGenericTypeConverter GenericTypeConverter { get; }
		#endregion

		#region Internal Properties
		internal FileInfo PhysicalFileInfo { get; }
		#endregion

		public bool IsNew { get; private set; }
		public string Name => PhysicalFileInfo.Name;
		public string SubPath { get; }
		public long Length => PhysicalFileInfo.Exists && !IsNew ? PhysicalFileInfo.Length : -1;
		public DateTimeOffset? LastModified => PhysicalFileInfo.Exists && !IsNew ? PhysicalFileInfo.LastWriteTimeUtc : (DateTimeOffset?)null;
		public string ContentType { get; }

		internal UmbrellaDiskFileInfo(ILogger<UmbrellaDiskFileInfo> logger,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			string subpath,
			UmbrellaDiskFileProvider provider,
			FileInfo physicalFileInfo,
			bool isNew)
		{
			Log = logger;
			Provider = provider;
			GenericTypeConverter = genericTypeConverter;
			PhysicalFileInfo = physicalFileInfo;
			IsNew = isNew;
			SubPath = subpath;

			ContentType = mimeTypeUtility.GetMimeType(Name);

			_metadataFullFileName = PhysicalFileInfo.FullName + ".meta";
		}

		#region IUmbrellaFileInfo Members
		public async Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var destinationFile = (UmbrellaDiskFileInfo)await Provider.CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
				File.Copy(PhysicalFileInfo.FullName, destinationFile.PhysicalFileInfo.FullName, true);

				destinationFile.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentOfType<UmbrellaDiskFileInfo>(destinationFile, nameof(destinationFile));

				if (!await ExistsAsync(cancellationToken))
					throw new UmbrellaFileNotFoundException(SubPath);

				var target = (UmbrellaDiskFileInfo)destinationFile;
				File.Copy(PhysicalFileInfo.FullName, target.PhysicalFileInfo.FullName, true);

				target.IsNew = false;

				return destinationFile;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { destinationFile }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				PhysicalFileInfo.Delete();
				File.Delete(_metadataFullFileName);

				return Task.FromResult(true);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				return Task.FromResult(PhysicalFileInfo.Exists);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfIsNew();
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

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

		public async Task WriteToStreamAsync(Stream target, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfIsNew();
				Guard.ArgumentNotNull(target, nameof(target));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				int bufferSize = bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize;

				using (var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true))
				{
					await fs.CopyToAsync(target, bufferSize, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public async Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

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

		public async Task WriteFromStreamAsync(Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNull(stream, nameof(stream));
				Guard.ArgumentInRange(bufferSizeOverride, nameof(bufferSizeOverride), 1, allowNull: true);

				if (!PhysicalFileInfo.Directory.Exists)
					PhysicalFileInfo.Directory.Create();

				int bufferSize = bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize;

				using (var fs = new FileStream(PhysicalFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write, bufferSize, true))
				{
					await stream.CopyToAsync(fs, bufferSize, cancellationToken).ConfigureAwait(false);
				}

				IsNew = false;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bufferSizeOverride }, returnValue: true))
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
				return await Task.FromResult(new FileStream(PhysicalFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSizeOverride ?? UmbrellaFileSystemConstants.SmallBufferSize, true)).ConfigureAwait(false);
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
				if (_metadataDictionary == null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				_metadataDictionary.TryGetValue(key, out string rawValue);

				return GenericTypeConverter.Convert(rawValue, fallback, customValueConverter);
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
				if (_metadataDictionary == null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				if (value == null)
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
				if (_metadataDictionary == null)
					await ReloadMetadataAsync(cancellationToken).ConfigureAwait(false);

				_metadataDictionary.Remove(key);

				if (writeChanges)
					await WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
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
				if (_metadataDictionary?.Count > 0)
				{
					string json = UmbrellaStatics.SerializeJson(_metadataDictionary);

					using (var fs = new FileStream(_metadataFullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true))
					{
						using (var sr = new StreamWriter(fs))
						{
							await sr.WriteAsync(json).ConfigureAwait(false);
						}
					}
				}
				else
				{
					File.Delete(_metadataFullFileName);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been an error writing the metadata changes.", exc);
			}
		}
		#endregion

		#region Private Methods
		private async Task ReloadMetadataAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfIsNew();

			try
			{
				string json = null;

				if (!File.Exists(_metadataFullFileName))
				{
					_metadataDictionary = new Dictionary<string, string>();
					return;
				}

				using (var fs = new FileStream(_metadataFullFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
				{
					using (var sr = new StreamReader(fs))
					{
						json = await sr.ReadToEndAsync().ConfigureAwait(false);
					}
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
				catch (Exception exc) when (Log.WriteError(exc, new { json }, "The JSON value stored in the Comment metadata property could not be deserialized to a Dictionary. This error has been handled silently.", returnValue: true))
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