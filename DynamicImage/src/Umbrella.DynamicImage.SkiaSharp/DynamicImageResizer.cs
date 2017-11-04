using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using System.Threading;
using SkiaSharp;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.SkiaSharp
{
    //TODO: Handle issues with transparency information being lost on resize
    public class DynamicImageResizer : DynamicImageResizerBase
    {
        #region Constructors
        public DynamicImageResizer(ILogger<DynamicImageResizer> logger,
            IDynamicImageCache dynamicImageCache)
            : base(logger, dynamicImageCache)
        {
        }
        #endregion

        #region Overridden Methods
        public override bool IsImage(byte[] bytes)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

                using (var image = LoadBitmap(bytes))
                {
                    return image != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public override byte[] ResizeImage(byte[] originalImage, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(originalImage, nameof(originalImage));

                using (var image = LoadBitmap(originalImage))
                {
                    SKBitmap imageToResize = image;

                    var result = GetDestinationDimensions(image.Width, image.Height, width, height, resizeMode);

                    try
                    {
                        if (result.OffsetX > 0 || result.OffsetY > 0)
                        {
                            var cropRect = SKRectI.Create(result.OffsetX, result.OffsetY, result.CropWidth, result.CropHeight);

                            imageToResize = new SKBitmap(cropRect.Width, cropRect.Height);
                            image.ExtractSubset(imageToResize, cropRect);
                        }

                        using (var resizedImage = imageToResize.Resize(new SKImageInfo(result.Width, result.Height), SKBitmapResizeMethod.Lanczos3))
                        {
                            using (var outputImage = SKImage.FromBitmap(resizedImage))
                            {
                                return outputImage.Encode(GetImageFormat(format), 75).ToArray();
                            }
                        }
                    }
                    finally
                    {
                        if (!ReferenceEquals(image, imageToResize))
                            imageToResize.Dispose();
                    }
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { width, height, resizeMode, format }, returnValue: true))
            {
                throw new DynamicImageException("An error has occurred during image resizing.", exc, width, height, resizeMode, format);
            }
        }
        #endregion

        #region Private Methods
        private SKBitmap LoadBitmap(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var s = new SKManagedStream(ms))
                {
                    using (var codec = SKCodec.Create(s))
                    {
                        var info = codec.Info;
                        var bitmap = new SKBitmap(new SKImageInfo(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul));

                        var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out IntPtr length));

                        if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                        {
                            return bitmap;
                        }
                        else
                        {
                            throw new ArgumentException("Unable to load bitmap from provided data");
                        }
                    }
                }
            }
        }

        private SKEncodedImageFormat GetImageFormat(DynamicImageFormat format)
        {
            switch (format)
            {
                case DynamicImageFormat.Bmp:
                    return SKEncodedImageFormat.Bmp;
                case DynamicImageFormat.Gif:
                    return SKEncodedImageFormat.Gif;
                case DynamicImageFormat.Jpeg:
                    return SKEncodedImageFormat.Jpeg;
                case DynamicImageFormat.Png:
                    return SKEncodedImageFormat.Png;
                default:
                    return default;
            }
        }
        #endregion
    }
}