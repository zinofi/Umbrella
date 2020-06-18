using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
	public interface IUmbrellaFileInfo
	{
		/// <summary>
		/// Gets a value indicating whether this file is new.
		/// </summary>
		bool IsNew { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the sub path relative to the file provider root, e.g. /images/my-house.png
		/// </summary>
		string SubPath { get; }

		/// <summary>
		/// Gets the length.
		/// </summary>
		long Length { get; }

		/// <summary>
		/// Gets the last modified date.
		/// </summary>
		DateTimeOffset? LastModified { get; }

		/// <summary>
		/// Gets the MIME type of the file content.
		/// </summary>
		string ContentType { get; }

		/// <summary>
		/// Checks if the current file exists using the file provider from which it was loaded.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> which will return <see langword="true" /> if it exists; otherwise <see langword="false"/>.</returns>
		Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
		Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null);
		Task WriteToStreamAsync(Stream target, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task WriteFromStreamAsync(Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task<bool> DeleteAsync(CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> MoveAsync(string destinationSubpath, CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> MoveAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default);
		Task<T> GetMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, T fallback = default, Func<string, T> customValueConverter = null);
		Task SetMetadataValueAsync<T>(string key, T value, CancellationToken cancellationToken = default, bool writeChanges = true);
		Task RemoveMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, bool writeChanges = true);
		Task ClearMetadataAsync<T>(CancellationToken cancellationToken = default, bool writeChanges = true);
		Task WriteMetadataChangesAsync(CancellationToken cancellationToken = default);
	}
}