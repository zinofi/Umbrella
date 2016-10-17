using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using System.Text.RegularExpressions;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImageUtility : IDynamicImageUtility
    {
        #region Private Static Members
        private static readonly Regex s_DensityRegex = new Regex("@([0-9]*)x$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

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

            //Check to see if the path has a density identifier at the end
            Match densityMatch = s_DensityRegex.Match(pathBuilder.ToString());

            if (densityMatch.Success)
            {
                //Get the density from the 2nd group
                if (densityMatch.Groups.Count == 2)
                {
                    int density = int.Parse(densityMatch.Groups[1].Value);
                    int densityIdentifierLength = densityMatch.Value.Length;

                    //Remove the density identifier from the path
                    pathBuilder.Remove(pathBuilder.Length - densityIdentifierLength, densityIdentifierLength);

                    //Double the dimensions
                    width = width * density;
                    height = height * density;
                }
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