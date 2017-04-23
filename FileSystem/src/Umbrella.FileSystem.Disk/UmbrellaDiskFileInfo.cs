using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.FileSystem.Disk
{
    public class UmbrellaDiskFileInfo : IUmbrellaFileInfo
    {
        public string Name => throw new NotImplementedException();
        public long Length => throw new NotImplementedException();
        public DateTimeOffset? LastModified => throw new NotImplementedException();

        public Task<bool> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default(CancellationToken), bool cacheContents = true)
        {
            throw new NotImplementedException();
        }

        public Task WriteFromByteArrayAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
