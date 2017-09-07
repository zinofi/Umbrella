using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
    public interface IUmbrellaFileProvider
    {
        Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string subpath, CancellationToken cancellationToken = default);
    }
}