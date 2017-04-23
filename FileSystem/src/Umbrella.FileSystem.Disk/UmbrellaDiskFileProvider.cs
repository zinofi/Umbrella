using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.FileSystem.Disk
{
    public class UmbrellaDiskFileProvider : IUmbrellaFileProvider
    {
        public Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
