using SoundInTheory.DynamicImage;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Fluent;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using UDynamicImageFormat = Umbrella.DynamicImage.Abstractions.DynamicImageFormat;
using SDynamicImageFormat = SoundInTheory.DynamicImage.DynamicImageFormat;
using System;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.SoundInTheory
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

                //Try and get an image from the bytes
                CompositionBuilder builder = new CompositionBuilder().WithLayer(LayerBuilder.Image.SourceBytes(bytes));

                //Trying to call this when we have a non-sensical image coming through will throw an exception
                GeneratedImage image = builder.Composition.GenerateImage();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override byte[] ResizeImage(byte[] originalImage, int width, int height, DynamicResizeMode resizeMode, UDynamicImageFormat format)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(originalImage, nameof(originalImage));

                ImageLayerBuilder imageLayerBuilder = LayerBuilder.Image.SourceBytes(originalImage);

                ResizeMode dynamicResizeMode = GetResizeMode(resizeMode);
                SDynamicImageFormat dynamicImageFormat = GetImageFormat(format);

                CompositionBuilder builder = new CompositionBuilder()
                    .WithLayer(imageLayerBuilder.WithFilter(FilterBuilder.Resize.To(width, height, dynamicResizeMode)))
                    .ImageFormat(dynamicImageFormat);

                GeneratedImage image = builder.Composition.GenerateImage();

                return ConvertBitmapSourceToByteArray(image.Image, format);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { width, height, resizeMode, format }, returnValue: true))
            {
                throw new Abstractions.DynamicImageException("An error has occurred during image resizing.", exc, width, height, resizeMode, format);
            }
        } 
        #endregion

        #region Private Methods
        private ResizeMode GetResizeMode(DynamicResizeMode mode)
        {
            switch (mode)
            {
                case DynamicResizeMode.Fill:
                    return ResizeMode.Fill;
                case DynamicResizeMode.Uniform:
                    return ResizeMode.Uniform;
                case DynamicResizeMode.UniformFill:
                    return ResizeMode.UniformFill;
                case DynamicResizeMode.UseHeight:
                    return ResizeMode.UseHeight;
                case DynamicResizeMode.UseWidth:
                    return ResizeMode.UseWidth;
                default:
                    return default;
            }
        }

        private SDynamicImageFormat GetImageFormat(UDynamicImageFormat format)
        {
            switch (format)
            {
                case UDynamicImageFormat.Bmp:
                    return SDynamicImageFormat.Bmp;
                case UDynamicImageFormat.Gif:
                    return SDynamicImageFormat.Gif;
                case UDynamicImageFormat.Jpeg:
                    return SDynamicImageFormat.Jpeg;
                case UDynamicImageFormat.Png:
                    return SDynamicImageFormat.Png;
                default:
                    return default;
            }
        }

        private byte[] ConvertBitmapSourceToByteArray(BitmapSource source, UDynamicImageFormat imageFormat)
        {
            BitmapFrame frame = BitmapFrame.Create(source);

            BitmapEncoder encoder = null;

            switch (imageFormat)
            {
                case UDynamicImageFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                case UDynamicImageFormat.Gif:
                    encoder = new GifBitmapEncoder();
                    break;
                case UDynamicImageFormat.Jpeg:
                    encoder = new JpegBitmapEncoder();
                    break;
                case UDynamicImageFormat.Png:
                    encoder = new PngBitmapEncoder();
                    break;
            }

            encoder.Frames.Add(frame);

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
        #endregion
    }
}