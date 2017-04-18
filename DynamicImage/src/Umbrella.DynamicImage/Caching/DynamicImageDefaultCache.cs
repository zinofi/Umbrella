using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage.Caching
{
    /// <summary>
    /// A default caching implementation that doesn't actually perform any caching. Useful for unit testing
    /// and scenarios where caching isn't required.
    /// </summary>
    public class DynamicImageDefaultCache : IDynamicImageCache
    {
        #region IDynamicImageCache Members
        public Task AddAsync(DynamicImageItem dynamicImage)
            => Task.CompletedTask;

        public Task<DynamicImageItem> GetAsync(string key, DateTime sourceLastModified, string fileExtension)
            => Task.FromResult<DynamicImageItem>(null);

        public Task RemoveAsync(string key, string fileExtension)
            => Task.CompletedTask;

        public string GenerateCacheKey(DynamicImageOptions options)
            => string.Empty; 
        #endregion
    }
}