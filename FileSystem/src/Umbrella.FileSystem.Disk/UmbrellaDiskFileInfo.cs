using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime;

namespace Umbrella.FileSystem.Disk
{
	//TODO: Override Equals, GetHashCode, etc to allow for equality comparisons
	public class UmbrellaDiskFileInfo : IUmbrellaFileInfo
	{
		#region Private Members
		private byte[] m_Contents;
		#endregion

		#region Protected Properties
		protected ILogger Log { get; }
		protected UmbrellaDiskFileProvider Provider { get; }
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
			string subpath,
			UmbrellaDiskFileProvider provider,
			FileInfo physicalFileInfo,
			bool isNew)
		{
			Log = logger;
			Provider = provider;
			PhysicalFileInfo = physicalFileInfo;
			IsNew = isNew;
			SubPath = subpath;

			ContentType = mimeTypeUtility.GetMimeType(Name);
		}

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

		#region Private Methods
		private void ThrowIfIsNew()
		{
			if (IsNew)
				throw new InvalidOperationException("Cannot read the contents of a newly created file. The file must first be written to.");
		}
		#endregion
	}
}