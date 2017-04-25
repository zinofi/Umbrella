using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
    public interface IUmbrellaFileInfo
    {
        bool IsNew { get; }
        string Name { get; }
        long Length { get; }
        DateTimeOffset? LastModified { get; }
        string ContentType { get; }
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default(CancellationToken), bool cacheContents = true);
        Task WriteFromByteArrayAsync(byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> CopyAsync(string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default(CancellationToken));
    }
}