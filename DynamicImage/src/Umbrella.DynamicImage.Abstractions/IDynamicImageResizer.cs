using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageResizer
    {
        Task<DynamicImageItem> GenerateImageAsync(Func<string, Task<(byte[] Bytes, DateTime SourceLastModified)>> sourceImageResolver, DynamicImageOptions options);
        Task<DynamicImageItem> GenerateImageAsync(byte[] bytes, DateTime sourceLastModified, DynamicImageOptions options);
    }
}