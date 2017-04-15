using SoundInTheory.DynamicImage;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using UDynamicImageFormat = Umbrella.DynamicImage.Abstractions.DynamicImageFormat;
using SDynamicImageFormat = SoundInTheory.DynamicImage.DynamicImageFormat;
using UDynamicImageException = Umbrella.DynamicImage.Abstractions.DynamicImageException;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.SoundInTheory
{
    public class DynamicImageResizer : IDynamicImageResizer
    {
        #region Private Members
        private readonly IDynamicImageCache m_DynamicImageCache;
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
        private readonly ILogger<DynamicImageResizer> m_Logger;
        #endregion

        #region Constructors
        public DynamicImageResizer(IDynamicImageCache dynamicImageCache,
            IUmbrellaHostingEnvironment hostingEnvironment,
            ILogger<DynamicImageResizer> logger)
        {
            m_DynamicImageCache = dynamicImageCache;
            m_HostingEnvironment = hostingEnvironment;
            m_Logger = logger;
        }
        #endregion

        #region IDynamicImageResizer Members
        public async Task<DynamicImageItem> GenerateImageAsync(Func<string, Task<(byte[] Bytes, DateTime SourceLastModified)>> sourceImageResolver, DynamicImageOptions options)
        {
            try
            {
                var (bytes, sourceLastModified) = await sourceImageResolver(options.SourcePath).ConfigureAwait(false);

                return await GenerateImageAsync(bytes, sourceLastModified, options).ConfigureAwait(false);
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { options }))
            {
                throw new UDynamicImageException("An error has occurred during image resizing.", exc, options);
            }
        }

        public async Task<DynamicImageItem> GenerateImageAsync(byte[] bytes, DateTime sourceLastModified, DynamicImageOptions options)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                    return null;

                string cacheKey = m_DynamicImageCache.GenerateCacheKey(options);

                //Check if the image exists in the cache
                DynamicImageItem dynamicImage = await m_DynamicImageCache.GetAsync(cacheKey, sourceLastModified, options.Format.ToFileExtensionString()).ConfigureAwait(false);

                if (dynamicImage != null)
                {
                    //Assign the options here as they aren't necessarily available when using
                    //anything other than the memory cache as the cache key which is based
                    //on the options is hashed one way.
                    dynamicImage.ImageOptions = options;
                    return dynamicImage;
                }

                //Item cannot be found in the cache - build a new image
                ImageLayerBuilder imageLayerBuilder = LayerBuilder.Image.SourceBytes(bytes);

                ResizeMode dynamicResizeMode = GetResizeMode(options.ResizeMode);
                SDynamicImageFormat dynamicImageFormat = GetImageFormat(options.Format);

                CompositionBuilder builder = new CompositionBuilder()
                    .WithLayer(imageLayerBuilder.WithFilter(FilterBuilder.Resize.To(options.Width, options.Height, dynamicResizeMode)))
                    .ImageFormat(dynamicImageFormat);

                GeneratedImage image = builder.Composition.GenerateImage();

                //Need to get the newly resized image and assign it to the instance
                dynamicImage = new DynamicImageItem
                {
                    ImageOptions = options,
                    LastModified = DateTime.UtcNow
                };

                dynamicImage.SetContent(ConvertBitmapSourceToByteArray(image.Image, options.Format));

                //Now add to the cache
                await m_DynamicImageCache.AddAsync(dynamicImage).ConfigureAwait(false);

                return dynamicImage;
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { sourceLastModified, options }))
            {
                throw new UDynamicImageException("An error has occurred during image resizing.", exc, options);
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
                    return default(ResizeMode);
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
                    return default(SDynamicImageFormat);
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