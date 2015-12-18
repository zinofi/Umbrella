using Umbrella.Utilities;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageUtility
    {
        #region Private Members
        private readonly IDynamicImageResizer m_DynamicImageResizer;
        private readonly IDynamicImageUrlGenerator m_DynamicImageUrlGenerator;
        #endregion

        #region Constructors
        public DynamicImageUtility(IDynamicImageResizer dynamicImageResizer,
            IDynamicImageUrlGenerator dynamicImageUrlGenerator)
        {
            m_DynamicImageResizer = dynamicImageResizer;
            m_DynamicImageUrlGenerator = dynamicImageUrlGenerator;
        }
        #endregion

        #region Public Methods
        public DynamicImage GetImage(int width, int height, DynamicResizeMode mode, string originalExtension, string path)
        {
            StringBuilder pathBuilder = new StringBuilder(path);

            //Replace the extension with the extension of the source file
            string extension = Path.GetExtension(path);
            pathBuilder.Remove(pathBuilder.Length - extension.Length, extension.Length);

            //Check to see if the path has an @2x at the end of the filename to indicate we need a retina version of the image
            //i.e. at 2x the requested resolution - Do this before adding the original extension
            if (pathBuilder.EndsWith("@2x"))
            {
                //Remove the @2x from the filename
                pathBuilder.Remove(pathBuilder.Length - 3, 3);

                //Double the dimensions
                width = width * 2;
                height = height * 2;
            }

            //Now add the original file extension to the path
            pathBuilder.Append("." + originalExtension);

            string updatedPath = pathBuilder.ToString().ToLower();

            return m_DynamicImageResizer.GenerateImage("~" + updatedPath, width, height, mode, ParseImageFormat(extension));
        }

        public string GetResizedUrl(string path, int width, int height, DynamicResizeMode mode, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            DynamicImageOptions options = new DynamicImageOptions
            {
                Format = format,
                Height = height,
                Mode = mode,
                OriginalVirtualPath = path,
                Width = width
            };

            return m_DynamicImageUrlGenerator.GenerateUrl(options, toAbsolutePath);
        }

        public DynamicImageFormat ParseImageFormat(string format)
        {
            switch (format)
            {
                case "png":
                    return DynamicImageFormat.Png;
                case "bmp":
                    return DynamicImageFormat.Bmp;
                default:
                case "jpg":
                    return DynamicImageFormat.Jpeg;
                case "gif":
                    return DynamicImageFormat.Gif;
            }
        }
        #endregion
    }
}