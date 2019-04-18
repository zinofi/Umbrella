using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
	// TODO: V3 Design - Consider implementing IFileProvider from the Microsoft.Extensions namespace for better compatibility
	// moving forwards. Not a breaking change so could do it as part of 2.x
    public interface IUmbrellaFileProvider
    {
        Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default);
        Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
        Task<IUmbrellaFileInfo> SaveAsync(string subpath, Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null);
        Task<bool> ExistsAsync(string subpath, CancellationToken cancellationToken = default);
    }
}