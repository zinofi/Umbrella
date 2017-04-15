using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageCache
    {
        Task AddAsync(DynamicImageItem dynamicImage);
        Task<DynamicImageItem> GetAsync(string key, DateTime sourceLastModified, string fileExtension);
        Task RemoveAsync(string key, string fileExtension);
        string GenerateCacheKey(DynamicImageOptions options);
    }
}