using Umbrella.Utilities;
using log4net;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
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
using Ninject;
using Umbrella.Utilities.Hosting;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageResizer : IDynamicImageResizer
    {
        #region Private Static Members
        private static readonly ILog Log = LogManager.GetLogger(typeof(DynamicImageResizer));
        #endregion

        #region Private Members
        private IDynamicImageCache m_DynamicImageCache;
        private IHostingEnvironment m_HostingEnvironment;
        #endregion

        #region Constructors
        public DynamicImageResizer()
            : this(null, null)
        {

        }

        public DynamicImageResizer(IDynamicImageCache dynamicImageCache, IHostingEnvironment hostingEnvironment)
        {
            m_DynamicImageCache = dynamicImageCache;
            m_HostingEnvironment = hostingEnvironment;

            //Fallback to DI container implementations
            if (m_DynamicImageCache == null)
                m_DynamicImageCache = LibraryBindings.DependencyResolver.Get<IDynamicImageCache>();

            if (m_HostingEnvironment == null)
                m_HostingEnvironment = LibraryBindings.DependencyResolver.Get<IHostingEnvironment>();
        } 
        #endregion

        #region IDynamicImageResizer Members
        public DynamicImage GenerateImage(string virtualPath, int width, int height, DynamicResizeMode resizeMode, Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat imageFormat = Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat.Jpeg)
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
                DynamicImage dynamicImage = m_DynamicImageCache.Get(cacheKey, physicalPath, imageFormat.ToFileExtensionString());

                if (dynamicImage != null)
                {
                    dynamicImage.ImageOptions = options;
                    return dynamicImage;
                }

                //Item cannot be found in the cache - build a new image
                
                ImageLayerBuilder imageLayerBuilder = LayerBuilder.Image.SourceFile(physicalPath);

                ResizeMode dynamicResizeMode = (ResizeMode)(int)resizeMode;
                SoundInTheory.DynamicImage.DynamicImageFormat dynamicImageFormat = (SoundInTheory.DynamicImage.DynamicImageFormat)(int)imageFormat;

                CompositionBuilder builder = new CompositionBuilder().WithLayer(imageLayerBuilder.WithFilter(FilterBuilder.Resize.To(width, height, dynamicResizeMode))).ImageFormat(dynamicImageFormat);

                GeneratedImage image = builder.Composition.GenerateImage();

                //Need to get the newly resized image and assign it to the instance
                dynamicImage = new DynamicImage();
                dynamicImage.ImageOptions = options;
                dynamicImage.Content = BitmapSourceToByte(image.Image, imageFormat);
                dynamicImage.LastModified = DateTime.Now;

                //Now add to the cache
                m_DynamicImageCache.Add(dynamicImage);

                return dynamicImage;
            }
            catch (Exception exc)
            {
                Log.Error(string.Format("GenerateImage({0}, {1}, {2}, {3}, {4}) failed", virtualPath, width, height, resizeMode, imageFormat), exc);
                throw;
            }
        } 
        #endregion

        #region Private Static Methods
        private static byte[] BitmapSourceToByte(BitmapSource source, Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat imageFormat)
        {
            BitmapFrame frame = BitmapFrame.Create(source);

            BitmapEncoder encoder = null;

            switch (imageFormat)
            {
                case Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                case Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat.Gif:
                    encoder = new GifBitmapEncoder();
                    break;
                case Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat.Jpeg:
                    encoder = new JpegBitmapEncoder();
                    break;
                case Umbrella.WebUtilities.DynamicImage.Enumerations.DynamicImageFormat.Png:
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