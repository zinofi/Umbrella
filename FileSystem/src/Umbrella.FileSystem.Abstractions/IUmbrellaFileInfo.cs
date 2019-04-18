using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
    // TODO: Add a new parameter to the end of each method called bufferSizeOverride.
	// Internally, each method will use the Small or Large buffer size defaults. At least then consumers have control.
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
    }
}