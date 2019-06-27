using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
	public interface IUmbrellaFileInfo
	{
		bool IsNew { get; }
		string Name { get; }
		string SubPath { get; }
		long Length { get; }
		DateTimeOffset? LastModified { get; }
		string ContentType { get; }
		Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
		Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default, bool cacheContents = true, int? bufferSizeOverride = null);
		Task WriteToStreamAsync(Stream target, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task WriteFromStreamAsync(Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
		Task<bool> DeleteAsync(CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default);
		Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default);
		Task<T> GetMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, T fallback = default, Func<string, T> customValueConverter = null);
		Task SetMetadataValueAsync<T>(string key, T value, CancellationToken cancellationToken = default, bool writeChanges = true);
		Task RemoveMetadataValueAsync<T>(string key, CancellationToken cancellationToken = default, bool writeChanges = true);
		Task WriteMetadataChangesAsync(CancellationToken cancellationToken = default);
	}
}