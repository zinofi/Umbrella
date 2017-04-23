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
        string Name { get; }
        long Length { get; }
        DateTimeOffset? LastModified { get; }
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default(CancellationToken), bool cacheContents = true);
        Task WriteFromByteArrayAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}