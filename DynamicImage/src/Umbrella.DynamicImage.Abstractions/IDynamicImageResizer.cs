using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageResizer
    {
        bool IsImage(byte[] bytes);
        Task<DynamicImageItem> GenerateImageAsync(IUmbrellaFileProvider sourceFileProvider, DynamicImageOptions options, CancellationToken cancellationToken = default);
        Task<DynamicImageItem> GenerateImageAsync(Func<Task<byte[]>> sourceBytesProvider, DateTimeOffset sourceLastModified, DynamicImageOptions options, CancellationToken cancellationToken = default);
        byte[] ResizeImage(byte[] originalImage, DynamicImageOptions options);
        byte[] ResizeImage(byte[] originalImage, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format);
    }
}