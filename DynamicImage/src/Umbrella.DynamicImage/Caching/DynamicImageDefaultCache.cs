using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using System.Threading;

namespace Umbrella.DynamicImage.Caching
{
    /// <summary>
    /// A default caching implementation that doesn't actually perform any caching. Useful for unit testing
    /// and scenarios where caching isn't required.
    /// </summary>
    public class DynamicImageDefaultCache : IDynamicImageCache
    {
        #region IDynamicImageCache Members
        public Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken))
            => Task.CompletedTask;

        public Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult<DynamicImageItem>(null);

        public Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
            => Task.CompletedTask;
        #endregion
    }
}