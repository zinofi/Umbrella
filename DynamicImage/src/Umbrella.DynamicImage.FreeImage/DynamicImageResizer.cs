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
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.FreeImage
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

                using (var inputStream = new MemoryStream(bytes))
                {
                    using (var image = FreeImageBitmap.FromStream(inputStream))
                    {
                        return image != null;
                    }
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

                using (var inputStream = new MemoryStream(originalImage))
                {
                    using (var image = FreeImageBitmap.FromStream(inputStream))
                    {
                        FreeImageBitmap imageToSave = image;

                        var result = GetDestinationDimensions(image.Width, image.Height, width, height, resizeMode);

                        try
                        {
                            if (result.OffsetX > 0 || result.OffsetY > 0)
                                imageToSave = image.Copy(new Rectangle(result.OffsetX, result.OffsetY, result.CropWidth, result.CropHeight));

                            imageToSave.Rescale(result.Width, result.Height, FREE_IMAGE_FILTER.FILTER_LANCZOS3);

                            using (var outputStream = new MemoryStream())
                            {
                                imageToSave.Save(outputStream, GetImageFormat(format), GetSaveFlags(format));
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
            catch (Exception exc) when (Log.WriteError(exc, new { width, height, resizeMode, format }, returnValue: true))
            {
                throw new DynamicImageException("An error has occurred during image resizing.", exc, width, height, resizeMode, format);
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