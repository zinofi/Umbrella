using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
    public interface IUmbrellaFileProvider
    {
        Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default(CancellationToken));
        Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, CancellationToken cancellationToken = default(CancellationToken));
        Task<(bool Exists, IUmbrellaFileInfo FileInfo)> ExistsAsync(string subpath, CancellationToken cancellationToken = default(CancellationToken));
    }
}