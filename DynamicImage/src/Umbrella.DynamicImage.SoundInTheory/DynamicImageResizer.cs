using SoundInTheory.DynamicImage;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using ADynamicImageFormat = Umbrella.DynamicImage.Abstractions.DynamicImageFormat;
using SDynamicImageFormat = SoundInTheory.DynamicImage.DynamicImageFormat;

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
        public DynamicImageItem GenerateImage(string virtualPath, int width, int height, DynamicResizeMode resizeMode, Abstractions.DynamicImageFormat imageFormat = ADynamicImageFormat.Jpeg)
        {
            try
            {
                string physicalPath = m_HostingEnvironment.MapPath(virtualPath);

                if (!File.Exists(physicalPath))
                    return null;

                DynamicImageOptions options = new DynamicImageOptions
                {
                    Width = width,
                    Height = height,
                    Mode = resizeMode,
                    Format = imageFormat,
                    OriginalVirtualPath = virtualPath
                };

                string cacheKey = m_DynamicImageCache.GenerateCacheKey(options);

                //Check if the image exists in the cache
                DynamicImageItem dynamicImage = m_DynamicImageCache.Get(cacheKey, physicalPath, imageFormat.ToFileExtensionString());

                if (dynamicImage != null)
                {
                    dynamicImage.ImageOptions = options;
                    return dynamicImage;
                }

                //Item cannot be found in the cache - build a new image

                ImageLayerBuilder imageLayerBuilder = LayerBuilder.Image.SourceFile(physicalPath);
                
                ResizeMode dynamicResizeMode = (ResizeMode)(int)resizeMode;
                SDynamicImageFormat dynamicImageFormat = (SDynamicImageFormat)(int)imageFormat;

                CompositionBuilder builder = new CompositionBuilder().WithLayer(imageLayerBuilder.WithFilter(FilterBuilder.Resize.To(width, height, dynamicResizeMode))).ImageFormat(dynamicImageFormat);

                GeneratedImage image = builder.Composition.GenerateImage();

                //Need to get the newly resized image and assign it to the instance
                dynamicImage = new DynamicImageItem()
                {
                    ImageOptions = options,
                    Content = BitmapSourceToByte(image.Image, imageFormat),
                    LastModified = DateTime.UtcNow
                };

                //Now add to the cache
                m_DynamicImageCache.Add(dynamicImage);

                return dynamicImage;
            }
            catch (Exception exc) when(m_Logger.WriteError(exc, new { virtualPath, width, height, resizeMode, imageFormat }))
            {
                throw;
            }
        }
        #endregion

        #region Private Static Methods
        private static byte[] BitmapSourceToByte(BitmapSource source, ADynamicImageFormat imageFormat)
        {
            BitmapFrame frame = BitmapFrame.Create(source);

            BitmapEncoder encoder = null;

            switch (imageFormat)
            {
                case ADynamicImageFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                case ADynamicImageFormat.Gif:
                    encoder = new GifBitmapEncoder();
                    break;
                case ADynamicImageFormat.Jpeg:
                    encoder = new JpegBitmapEncoder();
                    break;
                case ADynamicImageFormat.Png:
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