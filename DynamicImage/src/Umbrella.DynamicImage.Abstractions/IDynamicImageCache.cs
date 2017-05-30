using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageCache
    {
        Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken));
        Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default(CancellationToken));
    }
}