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
using FreeImageAPI;
using System.Drawing;

namespace Umbrella.DynamicImage.FreeImage
{
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
        protected override byte[] ResizeImage(byte[] originalImage, DynamicImageOptions options)
        {
            using (var inputStream = new MemoryStream(originalImage))
            {
                using (var image = FreeImageBitmap.FromStream(inputStream))
                {
                    FreeImageBitmap imageToSave = image;

                    var result = GetDestinationDimensions(image.Width, image.Height, options.Width, options.Height, options.ResizeMode);

                    try
                    {
                        if (result.OffsetX > 0 || result.OffsetY > 0)
                            imageToSave = image.Copy(new Rectangle(result.OffsetX, result.OffsetY, result.CropWidth, result.CropHeight));

                        imageToSave.Rescale(result.Width, result.Height, FREE_IMAGE_FILTER.FILTER_LANCZOS3);

                        using (var outputStream = new MemoryStream())
                        {
                            imageToSave.Save(outputStream, GetImageFormat(options.Format), GetSaveFlags(options.Format));
                            return outputStream.ToArray();
                        }
                    }
                    finally
                    {
                        if (!ReferenceEquals(image, imageToSave))
                            imageToSave.Dispose();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private FREE_IMAGE_FORMAT GetImageFormat(DynamicImageFormat format)
        {
            switch (format)
            {
                case DynamicImageFormat.Bmp:
                    return FREE_IMAGE_FORMAT.FIF_BMP;
                case DynamicImageFormat.Gif:
                    return FREE_IMAGE_FORMAT.FIF_GIF;
                case DynamicImageFormat.Jpeg:
                    return FREE_IMAGE_FORMAT.FIF_JPEG;
                case DynamicImageFormat.Png:
                    return FREE_IMAGE_FORMAT.FIF_PNG;
                default:
                    return default;
            }
        }

        private FREE_IMAGE_SAVE_FLAGS GetSaveFlags(DynamicImageFormat format)
        {
            switch (format)
            {
                case DynamicImageFormat.Jpeg:
                    return FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD | FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE;
                default:
                case DynamicImageFormat.Bmp:
                case DynamicImageFormat.Gif:
                    return default;
                case DynamicImageFormat.Png:
                    return FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
            }
        }
        #endregion
    }
}